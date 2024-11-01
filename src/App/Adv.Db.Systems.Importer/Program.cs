using System.Diagnostics;
using Adv.Db.Systems.Importer;

Console.Out.WriteLine($"Importing started {Utils.DateNow()}");
var stopwatch = Stopwatch.StartNew();

Console.WriteLine($"args: [{string.Join(", ", args.Length == 0 ? [] : args)}]");

var dataDir = args.Length == 0
    ? Utils.GetProjectRoot().GoToRepoRoot().GoIntoDataDir()
    : args[0];
dataDir.SetAsCurrentDirectory();

await UnpackingService.UnpackGzippedData();
await RepairDatasetService.RepairPopularityCsv();

Console.Out.WriteLine($"Importing finished {Utils.DateNow()}");
Console.Out.WriteLine($"Took: {stopwatch.Elapsed.Hours}h{stopwatch.Elapsed.Minutes}m{stopwatch.Elapsed.Seconds}s{stopwatch.Elapsed.Milliseconds}ms");