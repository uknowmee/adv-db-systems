using System.Diagnostics;
using Adv.Db.Systems.App;

await Console.Out.WriteLineAsync($"dbcli started - {Utils.DateNow()}");
await Console.Out.WriteLineAsync($"args: [{string.Join(", ", args.Length == 0 ? [] : args)}]");
await Console.Out.WriteLineAsync("press \"C\" to exit...");

using var tokenSource = new CancellationTokenSource();
using var background = new CancellationTokenSource();
var keyboard = Utils.RunKeyboardListener(background.Token, new Progress<ConsoleKey>(key => Utils.CancelOnC(key, tokenSource)));
var ticker = Utils.RunProgressBar(background.Token, new Progress<char>(c => Console.Out.Write($"\r{c}")));

var report = string.Empty;
try
{
    var stopwatch = Stopwatch.StartNew();

    var (querySummary, consoleOutput) = await args.RunTask(tokenSource.Token);

    report = $"{stopwatch.GetTaskDurationInfo()}" +
             $"{Environment.NewLine}" +
             $"{consoleOutput}";

    await QuerySummaryService.SaveQuerySummary(querySummary);
}
catch (OperationCanceledException)
{
    report = "sorry to see you going :(";
}
catch (InvalidOperationException e)
{
    report = e.Message;
}
catch (Exception e)
{
    Console.Out.WriteLine(e);
    report = "something went wrong...";
}
finally
{
    background.Cancel();
    await keyboard;
    await ticker;
    await Console.Out.WriteLineAsync($"\r{new string(' ', 30)}\r" +
                                     $"{Environment.NewLine}" +
                                     $"dbcli finished - {Utils.DateNow()}" +
                                     $"{Environment.NewLine}" +
                                     $"{report}"
    );
}