using System.Text.Json;

namespace Adv.Db.Systems.App;

public static class QuerySummaryService
{
    private static readonly string SaveDir = Path.Combine(Path.GetTempPath(), "5f602e2f-ef92-46fa-9fe1-b865163f7a9a");
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public static async Task SaveQuerySummary(QuerySummary querySummary)
    {
        try
        {
            var path = Path.Combine(SaveDir, querySummary.GetFileName());

            var directoryPath = Path.GetDirectoryName(path);
            if (directoryPath != null)
            {
                Directory.CreateDirectory(directoryPath);
            }

            await File.WriteAllTextAsync(path, JsonSerializer.Serialize(querySummary, Options));
        }
        catch (Exception)
        {
            // well, could not save...
        }
    }

    private static string GetFileName(this QuerySummary querySummary) => $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{querySummary.TaskName}.json";
}