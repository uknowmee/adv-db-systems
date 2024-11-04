using System.Diagnostics;
using Adv.Db.Systems.Importer;

Console.Out.WriteLine($"Importing started {Utils.DateNow()}");
var stopwatch = Stopwatch.StartNew();

Console.WriteLine($"args: [{string.Join(", ", args.Length == 0 ? [] : args)}]");

var dataDir = args.Length == 0
    ? DirectoryService.GetProjectRoot().GoToRepoRoot()
    : Directory.GetParent(args[0])?.FullName ?? DirectoryService.GetProjectRoot().GoToRepoRoot();
dataDir.SetAsCurrentDirectory();

await UnpackingService.UnpackGzippedDataAsync();

var fixPopularityTask = RepairDatasetService.RepairPopularityCsvAsync();
var getUniqueCategoriesTask = CategoriesService.GetUniqueCategoriesFromTaxonomiesAsync();

var (uniqueCategories, lines) = await getUniqueCategoriesTask;

var savePopularityToMemgraphTask = fixPopularityTask.ContinueWith(previous =>
    PopularityService.SavePopularityToMemgraphAcceptableCsvAsync(previous.Result).ContinueWith(_ =>
        MemgraphService.SavePopularityAsync()
    ).Unwrap()
).Unwrap();

var savePopularityRelationsToCsvTask = fixPopularityTask.ContinueWith(previous =>
    PopularityService.SavePopularityRelationsToMemgraphAcceptableCsvAsync(uniqueCategories, previous.Result)
).Unwrap();

var saveCategoriesToMemgraphTask = CategoriesService.SaveUniqueCategoriesToMemgraphAcceptableCsvAsync(uniqueCategories).ContinueWith(_ =>
    MemgraphService.SaveUniqueCategoriesAsync()
).Unwrap();

var saveCategoryRelationsToCsvTask = CategoriesService.GetCategoryRelationsFromTaxonomiesAsync(uniqueCategories, lines).ContinueWith(previous =>
    CategoriesService.SaveCategoryRelationsToMemgraphAcceptableCsvAsync(previous.Result)
).Unwrap();

var saveSubCategoriesToMemgraphTask = saveCategoriesToMemgraphTask.ContinueWith(_ =>
    saveCategoryRelationsToCsvTask.ContinueWith(_ => MemgraphService.SaveCategoriesRelationsAsync()).Unwrap()
).Unwrap();

var savePopularityRelationsToMemgraphTask = saveCategoriesToMemgraphTask.ContinueWith(_ =>
    savePopularityToMemgraphTask.ContinueWith(_ =>
        savePopularityRelationsToCsvTask.ContinueWith(_ =>
            MemgraphService.SavePopularityRelationsAsync()
        ).Unwrap()
    ).Unwrap()
).Unwrap();

await Task.WhenAll(saveSubCategoriesToMemgraphTask, savePopularityRelationsToMemgraphTask);

await MemgraphService.FireCreateCategoryNameIndex();

Console.Out.WriteLine($"Importing finished {Utils.DateNow()}");
Console.Out.WriteLine($"Took: {stopwatch.Elapsed.Hours}h{stopwatch.Elapsed.Minutes}m{stopwatch.Elapsed.Seconds}s{stopwatch.Elapsed.Milliseconds}ms");