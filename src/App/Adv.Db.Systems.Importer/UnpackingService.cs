using System.Diagnostics;
using System.IO.Compression;

namespace Adv.Db.Systems.Importer;

public static class UnpackingService
{
    public static async Task UnpackGzippedDataAsync()
    {
        await Console.Out.WriteLineAsync("Unpacking data");
        var stopwatch = Stopwatch.StartNew();

        var currentDir = Directory.GetCurrentDirectory();
        var compressedDir = Path.Combine(currentDir, DirectoryService.CompressedDataDir);
        var compressedFiles = Directory.GetFiles(compressedDir, "*.gz");

        foreach (var compressedFile in compressedFiles)
        {
            var fileName = Path.GetFileNameWithoutExtension(compressedFile);
            var uncompressedDir = Path.Combine(currentDir, fileName);

            await using var compressedStream = File.OpenRead(compressedFile);
            await using var decompressedStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            await using var fileStream = File.Create(uncompressedDir);

            await decompressedStream.CopyToAsync(fileStream);
        }

        await Console.Out.WriteLineAsync($"Unpacking done. {stopwatch.GetInfo()}");
    }
}