namespace Adv.Db.Systems.Importer;

internal static class Utils
{
    public static string DateNow()
    {
        return $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
    }

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

    public static string GoIntoDataDir(this string path)
    {
        return Path.Combine(path, "data");
    }

    public static string SetAsCurrentDirectory(this string path)
    {
        Directory.SetCurrentDirectory(path);
        return path;
    }
}