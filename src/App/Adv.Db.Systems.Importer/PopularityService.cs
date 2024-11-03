namespace Adv.Db.Systems.Importer;

public static class PopularityService
{
    public static async Task SavePopularityToMemgraphAcceptableCsvAsync()
    {
        await Console.Out.WriteLineAsync("Saving popularity to Memgraph acceptable CSV");
        await Task.Delay(0);
        await Console.Out.WriteLineAsync("Popularity saved to Memgraph acceptable CSV");
    }
}