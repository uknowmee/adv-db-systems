namespace Adv.Db.Systems.Importer;

public static class RepairDatasetService
{
    private const string TaxonomyFileDir = "taxonomy_iw.csv";
    private const string PopularityFileDir = "popularity_iw.csv";
    private const string PopularityFixedFileDir = "popularity_fixed.csv";
    private const string Splitter = ",";
    private const string TaxonomySplitter = "\",\"";
    private const string GoodEnding = "\"";
    private const char Trim = '"';

    private record Tuple(int Rating, List<string> Matches);

    public static async Task RepairPopularityCsv()
    {
        await Console.Out.WriteLineAsync("Repairing popularity csv");

        var uniqueCategoriesFromTaxonomy = GetUniqueCategoriesFromTaxonomies(TaxonomyFileDir);

        var badLinesFromPopularity = GetBadLinesFromPopularity(PopularityFileDir);
        var badPopularityRecords = GetBadPopularityRecords(badLinesFromPopularity);

        var goodLinesFromPopularity = GetGoodLinesFromPopularity(PopularityFileDir);
        var goodPopularityRecords = GetGoodPopularityRecords(goodLinesFromPopularity);

        var possiblyMisspelledCategories = GetPossiblyMisspelledCategories(goodPopularityRecords, uniqueCategoriesFromTaxonomy);
        var possiblyMisspelledCategoriesThatContainsComma = possiblyMisspelledCategories.Where(category => category.Contains(',')).ToHashSet();

        var misspelledCategoriesThatMatchesBadPopularityRecords = GetMisspelledCategoriesThatMatchesOtherKeys(
            possiblyMisspelledCategoriesThatContainsComma,
            badPopularityRecords
        );

        var oneMatchMisspelled = GetThoseWithOneMatch(misspelledCategoriesThatMatchesBadPopularityRecords);
        var merged = oneMatchMisspelled.Concat(goodPopularityRecords).ToDictionary();

        await SaveFixedPopularityCsv(PopularityFixedFileDir, merged);

        await Console.Out.WriteLineAsync("Repairing done");
    }

    private static HashSet<string> GetUniqueCategoriesFromTaxonomies(string taxonomyFileDir)
    {
        var uniqueValues = new HashSet<string>();
        using var reader = new StreamReader(taxonomyFileDir);

        while (reader.ReadLine() is { } line)
        {
            var firstComma = line.IndexOf(TaxonomySplitter, StringComparison.Ordinal);
            if (firstComma <= -1) continue;
            uniqueValues.Add(line[..firstComma].TrimOnce(Trim));
            uniqueValues.Add(line[(firstComma + 3)..].TrimOnce(Trim));
        }

        return uniqueValues;
    }

    private static HashSet<string> GetBadLinesFromPopularity(string popularityFileDir)
    {
        var badLines = new HashSet<string>();
        using var reader = new StreamReader(popularityFileDir);

        while (reader.ReadLine() is { } line)
        {
            var parts = line.Split(Splitter);
            var badPart = parts[0];

            if (badPart.EndsWith(GoodEnding) is false)
            {
                badLines.Add(line);
            }
        }

        return badLines;
    }

    private static HashSet<string> GetGoodLinesFromPopularity(string popularityFileDir)
    {
        var goodLines = new HashSet<string>();
        using var reader = new StreamReader(popularityFileDir);

        while (reader.ReadLine() is { } line)
        {
            var parts = line.Split(Splitter);
            var badPart = parts[0];

            if (badPart.EndsWith(GoodEnding))
            {
                goodLines.Add(line);
            }
        }

        return goodLines;
    }

    private static Dictionary<string, int> GetBadPopularityRecords(HashSet<string> badLinesFromPopularity)
    {
        var badPopularityRecords = new Dictionary<string, int>();
        foreach (var badLine in badLinesFromPopularity)
        {
            var parts = badLine.Split(Splitter);
            var key = parts[0].TrimOnce(Trim);
            var value = int.Parse(parts[1].TrimOnce(Trim));
            badPopularityRecords.Add(key, value);
        }

        return badPopularityRecords;
    }

    private static Dictionary<string, int> GetGoodPopularityRecords(HashSet<string> goodLinesFromPopularity)
    {
        var goodPopularityRecords = new Dictionary<string, int>();
        foreach (var badLine in goodLinesFromPopularity)
        {
            var parts = badLine.Split(Splitter);
            var key = parts[0].TrimOnce(Trim);
            var value = int.Parse(parts[1].TrimOnce(Trim));
            goodPopularityRecords.Add(key, value);
        }

        return goodPopularityRecords;
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

        return possiblyMisspelledCategories;
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
                oneMatchMisspelled.Add(tuple.Matches.First(), tuple.Rating);
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

    private static string TrimOnce(this string input, char trimChar)
    {
        if (input.StartsWith(trimChar))
        {
            input = input[1..];
        }

        if (input.EndsWith(trimChar))
        {
            input = input[..^1];
        }

        return input;
    }
}