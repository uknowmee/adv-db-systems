using System.Diagnostics;
using Neo4j.Driver;

namespace Adv.Db.Systems.Importer;

public static class MemgraphService
{
    private const string MemgraphUrl = "bolt://localhost:7687";

    private static readonly string CategoriesDir = DirectoryService.CategoriesDir.ToUnixPath();
    private static readonly string CategoryRelationsDir = DirectoryService.CategoryRelationsDir.ToUnixPath();

    private static readonly IDriver Driver = GraphDatabase.Driver(MemgraphUrl, AuthTokens.None);
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
                                       MATCH (sc:Category {id: row[2]})
                                       CREATE (c)-[:HAS_SUBCATEGORY]->(sc);
                                   """;
        var result = await Session.RunAsync(addCategoriesQuery);
        await result.ConsumeAsync();

        await Console.Out.WriteLineAsync($"Categories Relations inserted to Memgraph. {stopwatch.GetInfo()}");
    }

    public static async Task SavePopularityAsync()
    {
        await Console.Out.WriteLineAsync("Inserting Popularises to Memgraph");
        var stopwatch = Stopwatch.StartNew();
        await Task.Delay(0);
        await Console.Out.WriteLineAsync($"Popularises inserted to Memgraph. {stopwatch.GetInfo()}");
    }

    public static async Task SavePopularityRelationsAsync()
    {
        await Console.Out.WriteLineAsync("Inserting Popularises Relations to Memgraph");
        var stopwatch = Stopwatch.StartNew();
        await Task.Delay(0);
        await Console.Out.WriteLineAsync($"Popularises Relations inserted to Memgraph. {stopwatch.GetInfo()}");
    }
}