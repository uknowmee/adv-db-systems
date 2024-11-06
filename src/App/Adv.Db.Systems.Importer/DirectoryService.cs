namespace Adv.Db.Systems.Importer;

public static class DirectoryService
{
    public static string DataDir { get; set; } = "data";
    public static readonly string CompressedDataDir = Path.Combine(DataDir, "compressed");
    public static readonly string OriginalTaxonomyFileDir = Path.Combine(DataDir, "taxonomy_iw.csv");
    public static readonly string OriginalPopularityFileDir = Path.Combine(DataDir, "popularity_iw.csv");
    public static readonly string CategoriesDir = Path.Combine(DataDir, "categories.csv");
    public static readonly string CategoryRelationsDir = Path.Combine(DataDir, "categoryRelations.csv");
    public static readonly string PopularityDir = Path.Combine(DataDir, "popularity.csv");
    public static readonly string PopularityRelationsDir = Path.Combine(DataDir, "popularityRelations.csv");

    public static string GetProjectRoot()
    {
        // this usually works only in console apps.
        // AppDomain.CurrentDomain.BaseDirectory will return the directory where the .dll / .exe are located
        // usually when u deploy something with IIS / Azure / Docker ect. this will not work.
        // should work for both "dotnet run" and any "IDE run" scenarios

        var directory = AppDomain.CurrentDomain.BaseDirectory;
        var projectDirectory = Directory.GetParent(directory)?.Parent?.Parent?.Parent?.FullName ?? throw new InvalidOperationException();
        return projectDirectory;
    }

    public static string GoToRepoRoot(this string path)
    {
        return Directory.GetParent(path)?.Parent?.Parent?.FullName ?? throw new InvalidOperationException();
    }

    public static string SetAsCurrentDirectory(this string path)
    {
        Directory.SetCurrentDirectory(path);
        return path;
    }
}