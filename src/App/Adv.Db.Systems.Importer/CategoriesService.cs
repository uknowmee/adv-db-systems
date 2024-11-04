using System.Collections.Immutable;
using System.Diagnostics;

namespace Adv.Db.Systems.Importer;

public static class CategoriesService
{
    private const string TaxonomySplitter = "\",\"";
    private const string Backslash = "\\";
    private const string SingleQuote = "\"";

    public record CategoryInfo(int Id, string Name);

    public record CategoryRelation(CategoryInfo Category, CategoryInfo SubCategory);

    public static async Task<(ImmutableSortedDictionary<int, string>, ImmutableArray<(string Category, string SubCategory)>)> GetUniqueCategoriesFromTaxonomiesAsync()
    {
        await Console.Out.WriteLineAsync("Getting unique categories from taxonomies");
        var stopwatch = Stopwatch.StartNew();

        var uniqueValues = new HashSet<string>();
        var lines = new List<(string Category, string SubCategory)>();

        using var reader = new StreamReader(DirectoryService.TaxonomyFileDir);

        while (await reader.ReadLineAsync() is { } line)
        {
            line = line.Replace(Backslash, SingleQuote);
            var firstComma = line.IndexOf(TaxonomySplitter, StringComparison.Ordinal);
            if (firstComma <= -1) continue;

            var category = line.Substring(1, firstComma - 1);
            var subCategory = line.Substring(firstComma + 3, line.Length - firstComma - 4);

            uniqueValues.Add(category);
            uniqueValues.Add(subCategory);
            lines.Add((category, subCategory));
        }

        var result = uniqueValues
            .Select((value, index) => (value, index))
            .ToImmutableSortedDictionary(x => x.index, x => x.value);

        await Console.Out.WriteLineAsync($"Unique categories from taxonomies acquired {stopwatch.GetInfo()}");

        return (result, [..lines]);
    }

    public static async Task<ImmutableArray<CategoryRelation>> GetCategoryRelationsFromTaxonomiesAsync(
        ImmutableSortedDictionary<int, string> categories,
        ImmutableArray<(string Category, string SubCategory)> lines
    )
    {
        await Console.Out.WriteLineAsync("Getting category relations from taxonomies");
        var stopwatch = Stopwatch.StartNew();

        var categoryRelations = new List<CategoryRelation>();
        using var reader = new StreamReader(DirectoryService.TaxonomyFileDir);

        var categoryDict = categories.ToDictionary(x => x.Value, x => x.Key);

        foreach (var line in lines)
        {
            if (!categoryDict.TryGetValue(line.Category, out var categoryKey) || !categoryDict.TryGetValue(line.SubCategory, out var subCategoryKey))
            {
                continue;
            }

            var relation = new CategoryRelation(
                new CategoryInfo(categoryKey, line.Category),
                new CategoryInfo(subCategoryKey, line.SubCategory)
            );

            categoryRelations.Add(relation);
        }

        categoryRelations.Sort((x, y) =>
            {
                var categoryComparison = x.Category.Id.CompareTo(y.Category.Id);
                return categoryComparison != 0 ? categoryComparison : x.SubCategory.Id.CompareTo(y.SubCategory.Id);
            }
        );

        var immutableRelations = categoryRelations.ToImmutableArray();
        await Console.Out.WriteLineAsync($"Category relations from taxonomies acquired. {stopwatch.GetInfo()}");

        return immutableRelations;
    }

    public static async Task SaveUniqueCategoriesToMemgraphAcceptableCsvAsync(ImmutableSortedDictionary<int, string> categories)
    {
        await Console.Out.WriteLineAsync("Saving unique categories to Memgraph acceptable CSV");
        var stopwatch = Stopwatch.StartNew();

        await using var fileStream = new FileStream(DirectoryService.CategoriesDir, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
        await using var writer = new StreamWriter(fileStream);

        foreach (var (key, value) in categories)
        {
            await writer.WriteLineAsync($"{key},\"{value}\"");
        }

        await Console.Out.WriteLineAsync($"Unique categories saved to Memgraph acceptable CSV. {stopwatch.GetInfo()}");
    }

    public static async Task SaveCategoryRelationsToMemgraphAcceptableCsvAsync(ImmutableArray<CategoryRelation> categoryRelations)
    {
        await Console.Out.WriteLineAsync("Saving category relations to Memgraph acceptable CSV");
        var stopwatch = Stopwatch.StartNew();

        await using var fileStream = new FileStream(DirectoryService.CategoryRelationsDir, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
        await using var writer = new StreamWriter(fileStream);

        foreach (var relation in categoryRelations)
        {
            await writer.WriteLineAsync($"{relation.Category.Id},\"{relation.Category.Name}\",{relation.SubCategory.Id},\"{relation.SubCategory.Name}\"");
        }

        await Console.Out.WriteLineAsync($"Category relations saved to Memgraph acceptable CSV. {stopwatch.GetInfo()}");
    }
}