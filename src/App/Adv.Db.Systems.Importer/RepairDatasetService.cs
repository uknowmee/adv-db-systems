using System.Collections.Immutable;
using System.Diagnostics;

namespace Adv.Db.Systems.Importer;

public static class RepairDatasetService
{
    private const string Splitter = ",";
    private const string TaxonomySplitter = "\",\"";
    private const string GoodEnding = "\"";
    private const string SingleQuote = "\"";
    private const string DoubleQuote = "\"\"";
    private const char Trim = '"';

    private record Tuple(int Rating, List<string> Matches);

    public static async Task<ImmutableDictionary<string, int>> RepairPopularityCsvAsync()
    {
        await Console.Out.WriteLineAsync("Repairing popularity csv");
        var stopwatch = Stopwatch.StartNew();

        var uniqueCategoriesFromTaxonomyTask = GetUniqueCategoriesFromTaxonomiesAsync();
        var linesFromPopularityTask = GetLinesFromPopularityAsync();

        await Task.WhenAll(uniqueCategoriesFromTaxonomyTask, linesFromPopularityTask);
        var uniqueCategoriesFromTaxonomy = uniqueCategoriesFromTaxonomyTask.Result;
        var (badLinesFromPopularity, goodLinesFromPopularity) = linesFromPopularityTask.Result;

        var badPopularityRecordsTask = GetBadPopularityRecordsAsync(badLinesFromPopularity);
        var goodPopularityRecordsTask = GetGoodPopularityRecordsAsync(goodLinesFromPopularity);

        await Task.WhenAll(badPopularityRecordsTask, goodPopularityRecordsTask);
        var badPopularityRecords = badPopularityRecordsTask.Result;
        var goodPopularityRecords = goodPopularityRecordsTask.Result;

        var possiblyMisspelledCategoriesThatContainsComma = GetPossiblyMisspelledCategories(goodPopularityRecords, uniqueCategoriesFromTaxonomy);

        var misspelledCategoriesThatMatchesBadPopularityRecords = GetMisspelledCategoriesThatMatchesOtherKeys(
            possiblyMisspelledCategoriesThatContainsComma,
            badPopularityRecords
        );

        var oneMatchMisspelled = GetThoseWithOneMatch(misspelledCategoriesThatMatchesBadPopularityRecords);
        var popularity = ConcatAndFormat(oneMatchMisspelled, goodPopularityRecords);

        await Console.Out.WriteLineAsync($"Repairing done. {stopwatch.GetInfo()}");

        return popularity.ToImmutableDictionary();
    }

    private static async Task<HashSet<string>> GetUniqueCategoriesFromTaxonomiesAsync()
    {
        var uniqueValues = new HashSet<string>();
        using var reader = new StreamReader(DirectoryService.OriginalTaxonomyFileDir);

        while (await reader.ReadLineAsync() is { } line)
        {
            var firstComma = line.IndexOf(TaxonomySplitter, StringComparison.Ordinal);
            if (firstComma <= -1) continue;
            uniqueValues.Add(line.Substring(1, firstComma - 1));
            uniqueValues.Add(line.Substring(firstComma + 3, line.Length - firstComma - 4));
        }

        return uniqueValues;
    }

    private static async Task<(HashSet<string> badLines, HashSet<string> goodLines)> GetLinesFromPopularityAsync()
    {
        var badLines = new HashSet<string>();
        var goodLines = new HashSet<string>();

        using var reader = new StreamReader(DirectoryService.OriginalPopularityFileDir);
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

    private static Task<Dictionary<string, int>> GetBadPopularityRecordsAsync(HashSet<string> badLinesFromPopularity)
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

    private static Task<Dictionary<string, int>> GetGoodPopularityRecordsAsync(HashSet<string> goodLinesFromPopularity)
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

    private static Dictionary<string, int> ConcatAndFormat(Dictionary<string, int> oneMatchMisspelled, Dictionary<string, int> goodPopularityRecords)
    {
        var merged = oneMatchMisspelled.Concat(goodPopularityRecords).ToDictionary();
        merged = merged.ToDictionary(x => x.Key.Replace(SingleQuote, DoubleQuote), x => x.Value);

        return merged;
    }
}