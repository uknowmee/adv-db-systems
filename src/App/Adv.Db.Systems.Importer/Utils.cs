namespace Adv.Db.Systems.Importer;

internal static class Utils
{
    public static string DateNow()
    {
        return $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
    }

    public static string TrimOnce(this string input, char trimChar)
    {
        if (input.StartsWith(trimChar))
        {
            input = input[1..];
        }

        if (input.EndsWith(trimChar))
        {
            input = input[..^1];
        }

        return input;
    }

    public static string ToUnixPath(this string path)
    {
        return path.Replace('\\', '/');
    }
}