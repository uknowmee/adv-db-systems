using System.Collections.Immutable;
using Neo4j.Driver;

namespace Adv.Db.Systems.Importer;

public static class MemgraphCategoryInserter
{
    private const string TaxonomySplitter = "\",\"";
    private const string MemgraphUrl = "bolt://localhost:7687";

    private static readonly string CategoriesDir = DirectoryService.CategoriesDir.ToUnixPath();
    private static readonly string CategoryRelationsDir = DirectoryService.CategoryRelationsDir.ToUnixPath();

    private static readonly IDriver Driver = GraphDatabase.Driver(MemgraphUrl, AuthTokens.None);

    private record CategoryInfo(int Id, string Name);

    private record CategoryRelation(CategoryInfo Category, CategoryInfo SubCategory);

    public static async Task InsertTaxonomyDataAsync()
    {
        var categories = await SaveCategoriesToMemgraph();
        await SaveCategoriesRelationsToMemgraph(categories);
    }

    private static async Task<ImmutableSortedDictionary<int, string>> SaveCategoriesToMemgraph()
    {
        await Console.Out.WriteLineAsync("Preparing csv for Categories insertions");

        var categories = GetUniqueCategoriesFromTaxonomies();
        await SaveUniqueCategoriesToCsv(categories);

        await Console.Out.WriteLineAsync("Inserting Categories to Memgraph");

        var addCategoriesQuery = $$"""
                                       LOAD CSV FROM "{{CategoriesDir}}" NO HEADER AS row
                                       CREATE (n:Category {id: row[0], name: row[1]})
                                   """;

        await using var session = Driver.AsyncSession();
        await session.RunAsync(addCategoriesQuery);

        const string createIndexQuery = "CREATE INDEX ON :Category(id)";
        await session.RunAsync(createIndexQuery);

        await Console.Out.WriteLineAsync("Categories inserted to Memgraph");

        return categories;
    }

    private static ImmutableSortedDictionary<int, string> GetUniqueCategoriesFromTaxonomies()
    {
        var uniqueValues = new HashSet<string>();
        using var reader = new StreamReader(DirectoryService.TaxonomyFileDir);

        while (reader.ReadLine() is { } line)
        {
            line = line.Replace("\\", "\"");
            var firstComma = line.IndexOf(TaxonomySplitter, StringComparison.Ordinal);
            if (firstComma <= -1) continue;
            uniqueValues.Add(line.Substring(1, firstComma - 1));
            uniqueValues.Add(line.Substring(firstComma + 3, line.Length - firstComma - 4));
        }

        return uniqueValues
            .Select((value, index) => (value, index))
            .ToImmutableSortedDictionary(x => x.index, x => x.value);
    }

    private static async Task SaveUniqueCategoriesToCsv(ImmutableSortedDictionary<int, string> categories)
    {
        await using var fileStream = new FileStream(DirectoryService.CategoriesDir, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
        await using var writer = new StreamWriter(fileStream);

        foreach (var (key, value) in categories)
        {
            await writer.WriteLineAsync($"{key},\"{value}\"");
        }
    }

    private static async Task SaveCategoriesRelationsToMemgraph(ImmutableSortedDictionary<int, string> categories)
    {
        await Console.Out.WriteLineAsync("Preparing csv for Sub Categories insertions");

        var categoryRelations = GetCategoryRelationsFromTaxonomies(categories);
        await SaveCategoryRelationsToCsv(categoryRelations);

        await Console.Out.WriteLineAsync("Inserting Sub Categories to Memgraph");
        
        var addCategoriesQuery = $$"""
                                       LOAD CSV FROM "{{CategoryRelationsDir}}" NO HEADER AS row
                                       MATCH (c:Category {id: row[0]})
                                       MATCH (sc:Category {id: row[2]})
                                       CREATE (c)-[:HAS_SUBCATEGORY]->(sc);
                                   """;

        await using var session = Driver.AsyncSession();
        await session.RunAsync(addCategoriesQuery);
        
        await Console.Out.WriteLineAsync("Sub Categories inserted to Memgraph");
    }

    private static List<CategoryRelation> GetCategoryRelationsFromTaxonomies(ImmutableSortedDictionary<int, string> categories)
    {
        var categoryRelations = new List<CategoryRelation>();
        using var reader = new StreamReader(DirectoryService.TaxonomyFileDir);

        var categoryDict = categories.ToDictionary(x => x.Value, x => x.Key);

        while (reader.ReadLine() is { } line)
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

        return categoryRelations;
    }

    private static async Task SaveCategoryRelationsToCsv(List<CategoryRelation> categoryRelations)
    {
        await using var fileStream = new FileStream(CategoryRelationsDir, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
        await using var writer = new StreamWriter(fileStream);

        foreach (var relation in categoryRelations)
        {
            await writer.WriteLineAsync($"{relation.Category.Id},\"{relation.Category.Name}\",{relation.SubCategory.Id},\"{relation.SubCategory.Name}\"");
        }
    }
}