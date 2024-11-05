using System.Diagnostics;

namespace Adv.Db.Systems.App;

internal static class Utils
{
    public static string DateNow()
    {
        return $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
    }

    public static string GetTaskInfo(this Stopwatch stopwatch)
    {
        return $"Solving Task took: {stopwatch.Elapsed.Hours}h{stopwatch.Elapsed.Minutes}m{stopwatch.Elapsed.Seconds}s{stopwatch.Elapsed.Milliseconds}ms";
    }
    
    public static async Task RunProgress(CancellationToken token, IProgress<char> progress)
    {
        try
        {
            char[] states = ['|', '/', '-', '\\'];
            var state = 0;

            while (!token.IsCancellationRequested)
            {
                progress.Report(states[state]);
                state = (state + 1) % states.Length;
                await Task.Delay(100, token);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
    
    public static async Task RunKeyboardListener(CancellationToken token, IProgress<ConsoleKey> progress)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                if (Console.KeyAvailable)
                {
                    progress.Report(Console.ReadKey(true).Key);
                }
                else
                {
                    await Task.Delay(20, token);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
    
    public static async void CancelOnC(ConsoleKey key, CancellationTokenSource source)
    {
        if (key == ConsoleKey.C)
        {
            await source.CancelAsync();
        }
    }
}