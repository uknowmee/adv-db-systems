using System.IO.Compression;

namespace Adv.Db.Systems.Importer;

public static class UnpackingService
{
    private const string CompressedDataDir = "compressed";

    public static async Task UnpackGzippedData()
    {
        await Console.Out.WriteLineAsync("Unpacking data");

        var compressedDir = Path.Combine(Directory.GetCurrentDirectory(), CompressedDataDir);
        var compressedFiles = Directory.GetFiles(compressedDir, "*.gz");

        foreach (var compressedFile in compressedFiles)
        {
            var decompressedFile = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileNameWithoutExtension(compressedFile));
            await using var compressedStream = File.OpenRead(compressedFile);
            await using var decompressedStream = File.Create(decompressedFile);
            await using var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            await gzipStream.CopyToAsync(decompressedStream);
        }

        await Console.Out.WriteLineAsync("Unpacking done");
    }
}