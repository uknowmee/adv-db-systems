using System.Diagnostics;
using Neo4j.Driver;

namespace Adv.Db.Systems.Importer;

public static class MemgraphService
{
    private static readonly string MemgraphUri = Environment.GetEnvironmentVariable("MEMGRAPH_URI") ?? "bolt://localhost:7687";

    private static readonly string CategoriesDir = Path.Combine(DirectoryService.DataDir, DirectoryService.CategoriesDir).ToUnixPath();
    private static readonly string CategoryRelationsDir = Path.Combine(DirectoryService.DataDir, DirectoryService.CategoryRelationsDir).ToUnixPath();
    private static readonly string PopularityDir = Path.Combine(DirectoryService.DataDir, DirectoryService.PopularityDir).ToUnixPath();
    private static readonly string PopularityRelationsDir = Path.Combine(DirectoryService.DataDir, DirectoryService.PopularityRelationsDir).ToUnixPath();

    private static readonly IDriver Driver = GraphDatabase.Driver(MemgraphUri, AuthTokens.None);
    private static readonly IAsyncSession Session = Driver.AsyncSession();

    public static async Task SaveUniqueCategoriesAsync()
    {
        await Console.Out.WriteLineAsync("Inserting Categories to Memgraph");
        var stopwatch = Stopwatch.StartNew();

        const string createIndexQuery = "CREATE INDEX ON :Category(id)";
        var result = await Session.RunAsync(createIndexQuery);
        await result.ConsumeAsync();

        var addCategoriesQuery = $$"""
                                   USING PERIODIC COMMIT 1000000
                                   LOAD CSV FROM "{{CategoriesDir}}" NO HEADER AS row
                                   CREATE (n:Category {id: row[0], name: row[1]})
                                   """;
        result = await Session.RunAsync(addCategoriesQuery);
        await result.ConsumeAsync();

        await Console.Out.WriteLineAsync($"Categories inserted to Memgraph. {stopwatch.GetInfo()}");
    }

    public static async Task SaveCategoriesRelationsAsync()
    {
        await Console.Out.WriteLineAsync("Inserting Categories Relations to Memgraph");
        var stopwatch = Stopwatch.StartNew();

        var addCategoriesQuery = $$"""
                                       USING PERIODIC COMMIT 1000000
                                       LOAD CSV FROM "{{CategoryRelationsDir}}" NO HEADER AS row
                                       MATCH (c:Category {id: row[0]})
                                       MATCH (sc:Category {id: row[1]})
                                       CREATE (c)-[:HAS_SUBCATEGORY]->(sc);
                                   """;
        var result = await Session.RunAsync(addCategoriesQuery);
        await result.ConsumeAsync();

        await Console.Out.WriteLineAsync($"Categories Relations inserted to Memgraph. {stopwatch.GetInfo()}");
    }

    public static async Task SavePopularityAsync()
    {
        await Console.Out.WriteLineAsync("Inserting Popularity to Memgraph");
        var stopwatch = Stopwatch.StartNew();

        const string createIndexQuery = "CREATE INDEX ON :Popularity(id)";
        var result = await Session.RunAsync(createIndexQuery);
        await result.ConsumeAsync();

        var addPopularityQuery = $$"""
                                   USING PERIODIC COMMIT 100000
                                   LOAD CSV FROM "{{PopularityDir}}" NO HEADER AS row
                                   CREATE (n:Popularity {id: row[0]})
                                   """;

        result = await Session.RunAsync(addPopularityQuery);
        await result.ConsumeAsync();

        await Console.Out.WriteLineAsync($"Popularity inserted to Memgraph. {stopwatch.GetInfo()}");
    }

    public static async Task SavePopularityRelationsAsync()
    {
        await Console.Out.WriteLineAsync("Inserting Popularity Relations to Memgraph");
        var stopwatch = Stopwatch.StartNew();

        var addPopularityRelationsQuery = $$"""
                                            USING PERIODIC COMMIT 100000
                                            LOAD CSV FROM "{{PopularityRelationsDir}}" NO HEADER AS row
                                            MATCH (c:Category {id: row[0]})
                                            MATCH (p:Popularity {id: row[1]})
                                            CREATE (c)-[:HAS_POPULARITY]->(p)
                                            """;

        var result = await Session.RunAsync(addPopularityRelationsQuery);
        await result.ConsumeAsync();

        await Console.Out.WriteLineAsync($"Popularity Relations inserted to Memgraph. {stopwatch.GetInfo()}");
    }

    public static async Task FireCreateCategoryNameIndex()
    {
        const string createIndexQuery = "CREATE INDEX ON :Category(name)";
        await Session.RunAsync(createIndexQuery);
    }
}