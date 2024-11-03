using System.Diagnostics;
using Adv.Db.Systems.Importer;

Console.Out.WriteLine($"Importing started {Utils.DateNow()}");
var stopwatch = Stopwatch.StartNew();

Console.WriteLine($"args: [{string.Join(", ", args.Length == 0 ? [] : args)}]");

var dataDir = args.Length == 0
    ? DirectoryService.GetProjectRoot().GoToRepoRoot()
    : Directory.GetParent(args[0])?.FullName ?? DirectoryService.GetProjectRoot().GoToRepoRoot();
dataDir.SetAsCurrentDirectory();

await UnpackingService.UnpackGzippedData();

var fixPopularityTask = RepairDatasetService.RepairPopularityCsv();
var getUniqueCategoriesTask = CategoriesService.GetUniqueCategoriesFromTaxonomiesAsync();

var savePopularityToDbTask = fixPopularityTask.ContinueWith(_ =>
    PopularityService.SavePopularityToMemgraphAcceptableCsvAsync().ContinueWith(_ => MemgraphService.SavePopularityAsync()).Unwrap()
).Unwrap();

var uniqueCategories = await getUniqueCategoriesTask;

var saveCategoriesCsvTask = CategoriesService.SaveUniqueCategoriesToMemgraphAcceptableCsvAsync(uniqueCategories);
var saveCategoriesMemgraphTask = saveCategoriesCsvTask.ContinueWith(_ => MemgraphService.SaveUniqueCategoriesAsync()).Unwrap();

var getRelationsTask = CategoriesService.GetCategoryRelationsFromTaxonomiesAsync(uniqueCategories);
var saveRelationsCsvTask = getRelationsTask.ContinueWith(previous => CategoriesService.SaveCategoryRelationsToMemgraphAcceptableCsvAsync(previous.Result)).Unwrap();

var subCategoriesMemgraphTask = saveCategoriesMemgraphTask.ContinueWith(_ =>
    saveRelationsCsvTask.ContinueWith(_ => MemgraphService.SaveCategoriesRelationsAsync()).Unwrap()
).Unwrap();

var categoriesPopularityToDbTask = savePopularityToDbTask.ContinueWith(_ =>
    saveCategoriesMemgraphTask.ContinueWith(_ => MemgraphService.SavePopularityRelationsAsync()).Unwrap()
).Unwrap();

await Task.WhenAll(subCategoriesMemgraphTask, categoriesPopularityToDbTask);

Console.Out.WriteLine($"Importing finished {Utils.DateNow()}");
Console.Out.WriteLine($"Took: {stopwatch.Elapsed.Hours}h{stopwatch.Elapsed.Minutes}m{stopwatch.Elapsed.Seconds}s{stopwatch.Elapsed.Milliseconds}ms");