using System.Diagnostics;

namespace Adv.Db.Systems.Importer;

public static class RepairDatasetService
{
    private const string Splitter = ",";
    private const string TaxonomySplitter = "\",\"";
    private const string GoodEnding = "\"";
    private const char Trim = '"';

    private record Tuple(int Rating, List<string> Matches);

    public static async Task RepairPopularityCsv()
    {
        await Console.Out.WriteLineAsync("Repairing popularity csv");
        var stopwatch = Stopwatch.StartNew();

        var uniqueCategoriesFromTaxonomyTask = GetUniqueCategoriesFromTaxonomies();
        var linesFromPopularityTask = GetLinesFromPopularity();

        await Task.WhenAll(uniqueCategoriesFromTaxonomyTask, linesFromPopularityTask);
        var uniqueCategoriesFromTaxonomy = uniqueCategoriesFromTaxonomyTask.Result;
        var (badLinesFromPopularity, goodLinesFromPopularity) = linesFromPopularityTask.Result;

        var badPopularityRecordsTask = GetBadPopularityRecords(badLinesFromPopularity);
        var goodPopularityRecordsTask = GetGoodPopularityRecords(goodLinesFromPopularity);

        await Task.WhenAll(badPopularityRecordsTask, goodPopularityRecordsTask);
        var badPopularityRecords = badPopularityRecordsTask.Result;
        var goodPopularityRecords = goodPopularityRecordsTask.Result;

        var possiblyMisspelledCategoriesThatContainsComma = GetPossiblyMisspelledCategories(goodPopularityRecords, uniqueCategoriesFromTaxonomy);

        var misspelledCategoriesThatMatchesBadPopularityRecords = GetMisspelledCategoriesThatMatchesOtherKeys(
            possiblyMisspelledCategoriesThatContainsComma,
            badPopularityRecords
        );

        var oneMatchMisspelled = GetThoseWithOneMatch(misspelledCategoriesThatMatchesBadPopularityRecords);
        var merged = oneMatchMisspelled.Concat(goodPopularityRecords).ToDictionary();

        await SaveFixedPopularityCsv(DirectoryService.PopularityFixedFileDir, merged);

        await Console.Out.WriteLineAsync($"Repairing done. {stopwatch.GetInfo()}");
    }

    private static async Task<HashSet<string>> GetUniqueCategoriesFromTaxonomies()
    {
        var uniqueValues = new HashSet<string>();
        using var reader = new StreamReader(DirectoryService.TaxonomyFileDir);

        while (await reader.ReadLineAsync() is { } line)
        {
            var firstComma = line.IndexOf(TaxonomySplitter, StringComparison.Ordinal);
            if (firstComma <= -1) continue;
            uniqueValues.Add(line.Substring(1, firstComma - 1));
            uniqueValues.Add(line.Substring(firstComma + 3, line.Length - firstComma - 4));
        }

        return uniqueValues;
    }

    private static async Task<(HashSet<string> badLines, HashSet<string> goodLines)> GetLinesFromPopularity()
    {
        var badLines = new HashSet<string>();
        var goodLines = new HashSet<string>();

        using var reader = new StreamReader(DirectoryService.PopularityFileDir);
        while (await reader.ReadLineAsync() is { } line)
        {
            var parts = line.Split(Splitter);
            var badPart = parts[0];

            if (badPart.EndsWith(GoodEnding))
            {
                goodLines.Add(line);
            }
            else
            {
                badLines.Add(line);
            }
        }

        return (badLines, goodLines);
    }

    private static Task<Dictionary<string, int>> GetBadPopularityRecords(HashSet<string> badLinesFromPopularity)
    {
        var badPopularityRecords = new Dictionary<string, int>();
        foreach (var badLine in badLinesFromPopularity)
        {
            var parts = badLine.Split(Splitter);
            var key = parts[0].TrimOnce(Trim);
            var value = int.Parse(parts[1].TrimOnce(Trim));
            badPopularityRecords[key] = value;
        }

        return Task.FromResult(badPopularityRecords);
    }

    private static Task<Dictionary<string, int>> GetGoodPopularityRecords(HashSet<string> goodLinesFromPopularity)
    {
        var goodPopularityRecords = new Dictionary<string, int>();
        foreach (var goodLine in goodLinesFromPopularity)
        {
            var parts = goodLine.Split(Splitter);
            var key = parts[0].TrimOnce(Trim);
            var value = int.Parse(parts[1].TrimOnce(Trim));
            goodPopularityRecords[key] = value;
        }

        return Task.FromResult(goodPopularityRecords);
    }

    private static HashSet<string> GetPossiblyMisspelledCategories(Dictionary<string, int> goodPopularityRecords, HashSet<string> uniqueCategoriesFromTaxonomy)
    {
        var possiblyMisspelledCategories = new HashSet<string>();
        foreach (var category in uniqueCategoriesFromTaxonomy)
        {
            if (goodPopularityRecords.ContainsKey(category) is false)
            {
                possiblyMisspelledCategories.Add(category);
            }
        }

        return possiblyMisspelledCategories.Where(category => category.Contains(',')).ToHashSet();
    }

    private static Dictionary<string, Tuple> GetMisspelledCategoriesThatMatchesOtherKeys(
        HashSet<string> possiblyMisspelledCategories,
        Dictionary<string, int> badPopularityRecordsOther
    )
    {
        var misspelledCategoriesThatMatchesNumericKeys = new Dictionary<string, Tuple>();

        var prefixLookup = new Dictionary<string, List<string>>();
        foreach (var category in possiblyMisspelledCategories)
        {
            var prefix = category.Split(Splitter)[0];
            if (!prefixLookup.TryGetValue(prefix, out var value))
            {
                value = [];
                prefixLookup[prefix] = value;
            }

            value.Add(category);
        }

        foreach (var badRecord in badPopularityRecordsOther)
        {
            var matches = new List<string>();
            misspelledCategoriesThatMatchesNumericKeys[badRecord.Key] = new Tuple(badRecord.Value, matches);

            if (prefixLookup.TryGetValue(badRecord.Key, out var matchedCategories))
            {
                matches.AddRange(matchedCategories);
            }
        }

        return misspelledCategoriesThatMatchesNumericKeys;
    }

    private static Dictionary<string, int> GetThoseWithOneMatch(Dictionary<string, Tuple> misspelledCategoriesThatMatchesOtherKeys)
    {
        var oneMatchMisspelled = new Dictionary<string, int>();

        foreach (var (_, tuple) in misspelledCategoriesThatMatchesOtherKeys)
        {
            if (tuple.Matches.Count == 1)
            {
                oneMatchMisspelled[tuple.Matches.First()] = tuple.Rating;
            }
        }

        return oneMatchMisspelled;
    }

    private static async Task SaveFixedPopularityCsv(string fileDir, Dictionary<string, int> merged)
    {
        await using var fileStream = new FileStream(fileDir, FileMode.Create);
        await using var writer = new StreamWriter(fileStream);

        foreach (var (key, value) in merged)
        {
            await writer.WriteLineAsync($"\"{key}\",{value}");
        }
    }
}