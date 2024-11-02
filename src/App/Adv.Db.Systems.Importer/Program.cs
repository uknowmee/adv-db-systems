using System.Diagnostics;
using Adv.Db.Systems.Importer;

Console.Out.WriteLine($"Importing stages:" +
                      $"- Unpacking.{Environment.NewLine}" +
                      $"- Repairing popularity_iw.csv.{Environment.NewLine}" +
                      $"- Inserting Categories.{Environment.NewLine}" +
                      $"- Matching Categories Connections.{Environment.NewLine}" +
                      $"- Inserting Popularises.{Environment.NewLine}" +
                      $"- Matching Categories With Popularity.{Environment.NewLine}"
);

Console.Out.WriteLine($"Importing started {Utils.DateNow()}");
var stopwatch = Stopwatch.StartNew();

Console.WriteLine($"args: [{string.Join(", ", args.Length == 0 ? [] : args)}]");

var dataDir = args.Length == 0
    ? DirectoryService.GetProjectRoot().GoToRepoRoot()
    : Directory.GetParent(args[0])?.FullName ?? DirectoryService.GetProjectRoot().GoToRepoRoot();
dataDir.SetAsCurrentDirectory();

await UnpackingService.UnpackGzippedData();
await RepairDatasetService.RepairPopularityCsv();
await MemgraphCategoryInserter.InsertTaxonomyDataAsync();
await MemgraphPopularityInserter.InsertPopularity();

Console.Out.WriteLine($"Importing finished {Utils.DateNow()}");
Console.Out.WriteLine($"Took: {stopwatch.Elapsed.Hours}h{stopwatch.Elapsed.Minutes}m{stopwatch.Elapsed.Seconds}s{stopwatch.Elapsed.Milliseconds}ms");