using System.Collections.Immutable;

namespace Adv.Db.Systems.Importer;

public static class CategoriesService
{
    private const string TaxonomySplitter = "\",\"";

    public record CategoryInfo(int Id, string Name);

    public record CategoryRelation(CategoryInfo Category, CategoryInfo SubCategory);

    public static async Task<ImmutableSortedDictionary<int, string>> GetUniqueCategoriesFromTaxonomiesAsync()
    {
        await Console.Out.WriteLineAsync("Getting unique categories from taxonomies");
        
        var uniqueValues = new HashSet<string>();
        using var reader = new StreamReader(DirectoryService.TaxonomyFileDir);

        while (await reader.ReadLineAsync() is { } line)
        {
            line = line.Replace("\\", "\"");
            var firstComma = line.IndexOf(TaxonomySplitter, StringComparison.Ordinal);
            if (firstComma <= -1) continue;
            uniqueValues.Add(line.Substring(1, firstComma - 1));
            uniqueValues.Add(line.Substring(firstComma + 3, line.Length - firstComma - 4));
        }

        await Console.Out.WriteLineAsync("Unique categories from taxonomies acquired");
        
        return uniqueValues
            .Select((value, index) => (value, index))
            .ToImmutableSortedDictionary(x => x.index, x => x.value);
    }

    public static async Task<List<CategoryRelation>> GetCategoryRelationsFromTaxonomiesAsync(ImmutableSortedDictionary<int, string> categories)
    {
        await Console.Out.WriteLineAsync("Getting category relations from taxonomies");
        
        var categoryRelations = new List<CategoryRelation>();
        using var reader = new StreamReader(DirectoryService.TaxonomyFileDir);

        var categoryDict = categories.ToDictionary(x => x.Value, x => x.Key);

        while (await reader.ReadLineAsync() is { } line)
        {
            line = line.Replace("\\", "\"");
            var firstComma = line.IndexOf(TaxonomySplitter, StringComparison.Ordinal);
            if (firstComma <= -1) continue;

            var categoryName = line.Substring(1, firstComma - 1);
            var subCategoryName = line.Substring(firstComma + 3, line.Length - firstComma - 4);

            if (!categoryDict.TryGetValue(categoryName, out var categoryKey) || !categoryDict.TryGetValue(subCategoryName, out var subCategoryKey))
            {
                continue;
            }

            var relation = new CategoryRelation(
                new CategoryInfo(categoryKey, categoryName),
                new CategoryInfo(subCategoryKey, subCategoryName)
            );

            categoryRelations.Add(relation);
        }

        await Console.Out.WriteLineAsync("Category relations from taxonomies acquired");
        
        return categoryRelations;
    }

    public static async Task SaveUniqueCategoriesToMemgraphAcceptableCsvAsync(ImmutableSortedDictionary<int, string> categories)
    {
        await Console.Out.WriteLineAsync("Saving unique categories to Memgraph acceptable CSV");
        
        await using var fileStream = new FileStream(DirectoryService.CategoriesDir, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
        await using var writer = new StreamWriter(fileStream);

        foreach (var (key, value) in categories)
        {
            await writer.WriteLineAsync($"{key},\"{value}\"");
        }
        
        await Console.Out.WriteLineAsync("Unique categories saved to Memgraph acceptable CSV");
    }

    public static async Task SaveCategoryRelationsToMemgraphAcceptableCsvAsync(List<CategoryRelation> categoryRelations)
    {
        await Console.Out.WriteLineAsync("Saving category relations to Memgraph acceptable CSV");
        
        await using var fileStream = new FileStream(DirectoryService.CategoryRelationsDir, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
        await using var writer = new StreamWriter(fileStream);

        foreach (var relation in categoryRelations)
        {
            await writer.WriteLineAsync($"{relation.Category.Id},\"{relation.Category.Name}\",{relation.SubCategory.Id},\"{relation.SubCategory.Name}\"");
        }
        
        await Console.Out.WriteLineAsync("Category relations saved to Memgraph acceptable CSV");
    }
}