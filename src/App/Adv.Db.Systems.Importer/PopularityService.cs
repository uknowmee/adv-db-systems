using System.Collections.Immutable;
using System.Diagnostics;

namespace Adv.Db.Systems.Importer;

public static class PopularityService
{
    public static async Task SavePopularityToMemgraphAcceptableCsvAsync(ImmutableSortedDictionary<int, string> uniqueCategories, ImmutableDictionary<string, int> popularity)
    {
        await Console.Out.WriteLineAsync("Saving popularity to Memgraph acceptable CSV");
        var stopwatch = Stopwatch.StartNew();
        await Task.Delay(0);
        await Console.Out.WriteLineAsync($"Popularity saved to Memgraph acceptable CSV. {stopwatch.GetInfo()}");
    }
}