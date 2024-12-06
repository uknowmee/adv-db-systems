using System.Diagnostics;
using Neo4j.Driver;

namespace Adv.Db.Systems.App;

internal static class Utils
{
    public static string DateNow()
    {
        return $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}";
    }

    public static string GetTaskDurationInfo(this Stopwatch stopwatch)
    {
        return $"Solving Task took: {stopwatch.Elapsed.Hours}h{stopwatch.Elapsed.Minutes}m{stopwatch.Elapsed.Seconds}s{stopwatch.Elapsed.Milliseconds}ms";
    }

    public static async Task RunProgressBar(CancellationToken token, IProgress<char> progress)
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

    public static void Deconstruct(this TaskSummary taskSummary, out QuerySummary querySummary, out string consoleOutput)
    {
        querySummary = taskSummary.QuerySummary;
        consoleOutput = taskSummary.ConsoleOutput;
    }
    
    public static async Task<EagerResult<IReadOnlyList<TOut>>> RecordsAndSummary<TOut>(this Task<EagerResult<IReadOnlyList<TOut>>> eagerResultTask, CancellationToken tokenSource)
    {
        var completedTask = await Task.WhenAny(eagerResultTask, Task.Delay(Timeout.Infinite, tokenSource));

        if (completedTask != eagerResultTask)
        {
            throw new OperationCanceledException();
        }

        return await eagerResultTask;
    }
}