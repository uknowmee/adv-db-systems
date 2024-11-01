using System.Collections.Immutable;
using System.Diagnostics;

const string taxonomyFileDir = @"D:\projects\uknowmee\adv-db-systems\data\taxonomy_iw.csv";
const string popularityFileDir = @"D:\projects\uknowmee\adv-db-systems\data\popularity_iw.csv";
const string popularityFixedFileDir = @"D:\projects\uknowmee\adv-db-systems\data\popularity_fixed.csv";

//------------------------------------------------------------------------------------------------------------------------------

var uniqueCategoriesFromTaxonomy = Utils.GetUniqueCategoriesFromTaxonomies(taxonomyFileDir);
Utils.PrintEleventhElement(uniqueCategoriesFromTaxonomy, nameof(uniqueCategoriesFromTaxonomy));

//------------------------------------------------------------------------------------------------------------------------------

var badLinesFromPopularity = Utils.GetBadLinesFromPopularity(popularityFileDir);
Utils.PrintEleventhElement(badLinesFromPopularity, nameof(badLinesFromPopularity));

var badPopularityRecords = Utils.GetBadPopularityRecords(badLinesFromPopularity);
Utils.PrintEleventhElement(badPopularityRecords, nameof(badPopularityRecords));

//------------------------------------------------------------------------------------------------------------------------------

var goodLinesFromPopularity = Utils.GetGoodLinesFromPopularity(popularityFileDir);
Utils.PrintEleventhElement(goodLinesFromPopularity, nameof(goodLinesFromPopularity));

var goodPopularityRecords = Utils.GetGoodPopularityRecords(goodLinesFromPopularity);
Utils.PrintEleventhElement(goodPopularityRecords, nameof(goodPopularityRecords));

//------------------------------------------------------------------------------------------------------------------------------

var possiblyMisspelledCategories = Utils.GetPossiblyMisspelledCategories(goodPopularityRecords, uniqueCategoriesFromTaxonomy);
Utils.PrintEleventhElement(possiblyMisspelledCategories, nameof(possiblyMisspelledCategories));

//------------------------------------------------------------------------------------------------------------------------------

var possiblyMisspelledCategoriesThatContainsComma = possiblyMisspelledCategories.Where(category => category.Contains(',')).ToHashSet();
Console.Out.WriteLine($"{nameof(possiblyMisspelledCategoriesThatContainsComma)}: {possiblyMisspelledCategoriesThatContainsComma.Count}");

var misspelledCategoriesThatMatchesOtherKeys = Utils.GetMisspelledCategoriesThatMatchesOtherKeys(
    possiblyMisspelledCategoriesThatContainsComma,
    badPopularityRecords
);
Console.Out.WriteLine($"{nameof(misspelledCategoriesThatMatchesOtherKeys)}: {misspelledCategoriesThatMatchesOtherKeys.Count}");

//------------------------------------------------------------------------------------------------------------------------------
// dev

// const string misspelledCategoriesThatMatchesOtherKeysFileDir = @"D:\projects\uknowmee\adv-db-systems\data\map.json";
//
// await using var fileStream = new FileStream(misspelledCategoriesThatMatchesOtherKeysFileDir, FileMode.Create);
// await JsonSerializer.SerializeAsync(fileStream, misspelledCategoriesThatMatchesOtherKeys, new JsonSerializerOptions { WriteIndented = true });
//
// var misspelledCategoriesThatMatchesOtherKeys =
//     await JsonSerializer.DeserializeAsync<Dictionary<string, Tuple>>(new FileStream(misspelledCategoriesThatMatchesOtherKeysFileDir, FileMode.Open))
//     ?? new Dictionary<string, Tuple>();

//------------------------------------------------------------------------------------------------------------------------------

var matchesCount = Utils.CountMatches(misspelledCategoriesThatMatchesOtherKeys);
Console.Out.WriteLine($"{nameof(matchesCount)} | matches, matched records: " +
                      $"{string.Join(", ", matchesCount)}"
);

//------------------------------------------------------------------------------------------------------------------------------

var oneMatchMisspelled = Utils.GetThoseWithOneMatch(misspelledCategoriesThatMatchesOtherKeys);
var merged = oneMatchMisspelled.Concat(goodPopularityRecords).ToDictionary();

await Utils.SaveFixedPopularityCsv(popularityFixedFileDir, merged);

//------------------------------------------------------------------------------------------------------------------------------


return;

internal record Tuple(int Rating, List<string> Matches);

internal static class Utils
{
    private const string Splitter = ",";
    private const string GoodEnding = "\"";

    public static void PrintEleventhElement(Dictionary<string, int> toPrint, string varName)
    {
        Console.Out.WriteLine($"{varName}: {toPrint.Count}");
        Console.Out.WriteLine($"11th element: {toPrint.Skip(10).First()}");
        Console.Out.WriteLine("");
    }

    public static void PrintEleventhElement(HashSet<string> toPrint, string varName)
    {
        Console.Out.WriteLine($"{varName}: {toPrint.Count}");
        Console.Out.WriteLine($"11th element: {toPrint.Skip(10).First()}");
        Console.Out.WriteLine("");
    }

    public static HashSet<string> GetUniqueCategoriesFromTaxonomies(string taxonomyFileDir)
    {
        var uniqueValues = new HashSet<string>();
        using var reader = new StreamReader(taxonomyFileDir);

        while (reader.ReadLine() is { } line)
        {
            var firstComma = line.IndexOf("\",\"", StringComparison.Ordinal);
            if (firstComma <= -1) continue;
            uniqueValues.Add(line[..firstComma].Replace("\"", ""));
            uniqueValues.Add(line[(firstComma + 3)..].Replace("\"", ""));
        }

        return uniqueValues;
    }

    public static HashSet<string> GetBadLinesFromPopularity(string popularityFileDir)
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

    public static HashSet<string> GetGoodLinesFromPopularity(string popularityFileDir)
    {
        var goodLines = new HashSet<string>();
        using var reader = new StreamReader(popularityFileDir);

        while (reader.ReadLine() is { } line)
        {
            var parts = line.Split(Splitter);
            var badPart = parts[0];

            if (badPart.EndsWith(GoodEnding) is true)
            {
                goodLines.Add(line);
            }
        }

        return goodLines;
    }

    public static Dictionary<string, int> GetBadPopularityRecords(HashSet<string> badLinesFromPopularity)
    {
        var badPopularityRecords = new Dictionary<string, int>();
        foreach (var badLine in badLinesFromPopularity)
        {
            var parts = badLine.Split(",");
            var key = parts[0].Replace("\"", "");
            var value = int.Parse(parts[1].Replace("\"", ""));
            badPopularityRecords.Add(key, value);
        }

        return badPopularityRecords;
    }

    public static Dictionary<string, int> GetGoodPopularityRecords(HashSet<string> goodLinesFromPopularity)
    {
        var goodPopularityRecords = new Dictionary<string, int>();
        foreach (var badLine in goodLinesFromPopularity)
        {
            var parts = badLine.Split(",");
            var key = parts[0].Replace("\"", "");
            var value = int.Parse(parts[1].Replace("\"", ""));
            goodPopularityRecords.Add(key, value);
        }

        return goodPopularityRecords;
    }

    public static HashSet<string> GetPossiblyMisspelledCategories(Dictionary<string, int> goodPopularityRecords, HashSet<string> uniqueCategoriesFromTaxonomy)
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

    public static Dictionary<string, Tuple> GetMisspelledCategoriesThatMatchesOtherKeys(
        HashSet<string> possiblyMisspelledCategories,
        Dictionary<string, int> badPopularityRecordsOther
    )
    {
        var stopwatch = Stopwatch.StartNew();
        var misspelledCategoriesThatMatchesNumericKeys = new Dictionary<string, Tuple>();

        Console.Out.WriteLine("Starting matching.");

        var progress = 0;
        var total = badPopularityRecordsOther.Count;
        foreach (var badRecord in badPopularityRecordsOther)
        {
            progress++;
            if (progress % 100 == 0)
            {
                Console.Out.Write($"\rProgress: {(double)progress / total * 100}");
            }

            misspelledCategoriesThatMatchesNumericKeys.Add(badRecord.Key, new Tuple(badRecord.Value, []));

            foreach (var possiblyMisspelledCategory in possiblyMisspelledCategories)
            {
                if (possiblyMisspelledCategory.StartsWith(badRecord.Key + ","))
                {
                    misspelledCategoriesThatMatchesNumericKeys[badRecord.Key].Matches.Add(possiblyMisspelledCategory);
                }
            }
        }

        stopwatch.Stop();
        Console.Out.WriteLine("");
        Console.Out.WriteLine("Finished matching.");
        Console.Out.WriteLine($"Took: {stopwatch.Elapsed.Hours}h{stopwatch.Elapsed.Minutes}m{stopwatch.Elapsed.Seconds}s");
        return misspelledCategoriesThatMatchesNumericKeys;
    }

    public static IDictionary<int, int> CountMatches(Dictionary<string, Tuple> misspelledCategoriesThatMatchesOtherKeys)
    {
        var matchesCount = new Dictionary<int, int>();
        foreach (var (_, tuple) in misspelledCategoriesThatMatchesOtherKeys)
        {
            if (!matchesCount.TryAdd(tuple.Matches.Count, 1))
            {
                matchesCount[tuple.Matches.Count]++;
            }
        }

        return matchesCount.OrderBy(m => m.Key).ToImmutableSortedDictionary();
    }

    public static Dictionary<string, int> GetThoseWithOneMatch(Dictionary<string, Tuple> misspelledCategoriesThatMatchesOtherKeys)
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

    public static async Task SaveFixedPopularityCsv(string fileDir, Dictionary<string, int> merged)
    {
        await using var fileStream = new FileStream(fileDir, FileMode.Create);
        await using var writer = new StreamWriter(fileStream);

        foreach (var (key, value) in merged)
        {
            await writer.WriteLineAsync($"\"{key}\",{value}");
        }
    }
}