using System.Collections.Immutable;
using System.Diagnostics;

namespace Adv.Db.Systems.Importer;

public static class PopularityService
{
    public static async Task SavePopularityToMemgraphAcceptableCsvAsync(
        ImmutableDictionary<string, int> popularity
    )
    {
        await Console.Out.WriteLineAsync("Saving popularity to Memgraph acceptable CSV");
        var stopwatch = Stopwatch.StartNew();

        var popularityIds = popularity.Values.Distinct().Order();

        await using var fileStream = new FileStream(DirectoryService.PopularityDir, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
        await using var writer = new StreamWriter(fileStream);

        foreach (var id in popularityIds)
        {
            await writer.WriteLineAsync($"{id}");
        }

        await Console.Out.WriteLineAsync($"Popularity saved to Memgraph acceptable CSV. {stopwatch.GetInfo()}");
    }

    public static async Task SavePopularityRelationsToMemgraphAcceptableCsvAsync(
        ImmutableSortedDictionary<int, string> uniqueCategories,
        ImmutableDictionary<string, int> popularity
    )
    {
        await Console.Out.WriteLineAsync("Saving popularity relations to Memgraph acceptable CSV");
        var stopwatch = Stopwatch.StartNew();

        var categoryDict = uniqueCategories.ToDictionary(x => x.Value, x => x.Key);

        var orderedPopularity = popularity
            .Select((kvp, _) =>
                {
                    var (categoryName, popularityValue) = kvp;
                    var categoryId = categoryDict.GetValueOrDefault(categoryName, -1);
                    return (categoryId, popularityValue);
                }
            )
            .OrderBy(p => p.popularityValue)
            .ThenBy(p => p.categoryId)
            .ToImmutableArray();

        await using var fileStream = new FileStream(DirectoryService.PopularityRelationsDir, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
        await using var writer = new StreamWriter(fileStream);

        foreach (var record in orderedPopularity)
        {
            await writer.WriteLineAsync($"{record.categoryId},{record.popularityValue}");
        }

        await Console.Out.WriteLineAsync($"Popularity realtions saved to Memgraph acceptable CSV. {stopwatch.GetInfo()}");
    }
}