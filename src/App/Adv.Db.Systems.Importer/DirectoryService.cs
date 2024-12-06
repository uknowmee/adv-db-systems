namespace Adv.Db.Systems.Importer;

public static class DirectoryService
{
    public static string DataDir { get; set; } = "data";
    public const string CompressedDataDir = "compressed";
    public const string OriginalTaxonomyFileDir = "taxonomy_iw.csv";
    public const string OriginalPopularityFileDir = "popularity_iw.csv";
    public const string CategoriesDir = "categories.csv";
    public const string CategoryRelationsDir = "categoryRelations.csv";
    public const string PopularityDir = "popularity.csv";
    public const string PopularityRelationsDir = "popularityRelations.csv";

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