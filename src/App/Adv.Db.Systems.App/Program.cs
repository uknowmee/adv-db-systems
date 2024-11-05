using System.Diagnostics;
using Adv.Db.Systems.App;

await Console.Out.WriteLineAsync($"dbcli started - {Utils.DateNow()}");
await Console.Out.WriteLineAsync($"args: [{string.Join(", ", args.Length == 0 ? [] : args)}]");
await Console.Out.WriteLineAsync("press \"C\" to exit...");

using var tokenSource = new CancellationTokenSource();
using var background = new CancellationTokenSource();
var keyboard = Utils.RunKeyboardListener(background.Token, new Progress<ConsoleKey>(key => Utils.CancelOnC(key, tokenSource)));
var ticker = Utils.RunProgress(background.Token, new Progress<char>(c => Console.Out.Write($"\r{c}")));

var report = string.Empty;
try
{
    var stopwatch = Stopwatch.StartNew();

    await args.GetTask(tokenSource.Token);

    report = $"{stopwatch.GetTaskInfo()}";
}
catch (OperationCanceledException)
{
    report = "sorry to see you going :(";
}
catch (InvalidOperationException e)
{
    report = e.Message;
}
catch (Exception)
{
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