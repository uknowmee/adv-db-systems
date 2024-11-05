namespace Adv.Db.Systems.App;

public class MemgraphService
{
    private readonly CancellationToken _tokenSource;

    public MemgraphService(CancellationToken tokenSource)
    {
        _tokenSource = tokenSource;
    }

    public async Task Task01(string nodeName)
    {
        await Task.Delay(5000, _tokenSource);
        await Console.Out.WriteAsync($"{nodeName}");
    }

    public async Task Task02(string nodeName)
    {
        await Task.Delay(5000, _tokenSource);
        await Console.Out.WriteAsync($"{nodeName}");
    }

    public async Task Task03(string nodeName)
    {
        await Task.Delay(5000, _tokenSource);
        await Console.Out.WriteAsync($"{nodeName}");
    }

    public async Task Task04(string nodeName)
    {
        await Task.Delay(5000, _tokenSource);
        await Console.Out.WriteAsync($"{nodeName}");
    }

    public async Task Task05(string nodeName)
    {
        await Task.Delay(5000, _tokenSource);
        await Console.Out.WriteAsync($"{nodeName}");
    }

    public async Task Task06(string nodeName)
    {
        await Task.Delay(5000, _tokenSource);
        await Console.Out.WriteAsync($"{nodeName}");
    }

    public async Task Task07()
    {
        await Task.Delay(5000, _tokenSource);
        await Console.Out.WriteAsync($"task07");
    }

    public async Task Task08()
    {
        await Task.Delay(5000, _tokenSource);
        await Console.Out.WriteAsync($"task08");
    }

    public async Task Task09()
    {
        await Task.Delay(5000, _tokenSource);
        await Console.Out.WriteAsync($"task09");
    }

    public async Task Task10(int limit)
    {
        await Task.Delay(5000, _tokenSource);
        await Console.Out.WriteAsync($"{limit}");
    }

    public async Task Task11(int limit)
    {
        await Task.Delay(5000, _tokenSource);
        await Console.Out.WriteAsync($"{limit}");
    }

    public async Task Task12(string oldNodeName, string newNodeName)
    {
        await Task.Delay(5000, _tokenSource);
        await Console.Out.WriteAsync($"{oldNodeName}, {newNodeName}");
    }

    public async Task Task13(string nodeName, int newNodePopularity)
    {
        await Task.Delay(5000, _tokenSource);
        await Console.Out.WriteAsync($"{nodeName}, {newNodePopularity}");
    }

    public async Task Task14(string firstNodeName, string secondNodeName, int numberOfHops)
    {
        await Task.Delay(5000, _tokenSource);
        await Console.Out.WriteAsync($"{firstNodeName}, {secondNodeName}, {numberOfHops}");
    }

    public async Task Task15(string firstNodeName, string secondNodeName, int numberOfHops)
    {
        await Task.Delay(5000, _tokenSource);
        await Console.Out.WriteAsync($"{firstNodeName}, {secondNodeName}, {numberOfHops}");
    }

    public async Task Task16(string nodeName, int radius)
    {
        await Task.Delay(5000, _tokenSource);
        await Console.Out.WriteAsync($"{nodeName}, {radius}");
    }

    public async Task Task17(string firstNodeName, string secondNodeName)
    {
        await Task.Delay(5000, _tokenSource);
        await Console.Out.WriteAsync($"{firstNodeName}, {secondNodeName}");
    }

    public async Task Task18(string firstNodeName, string secondNodeName, int numberOfHops)
    {
        await Task.Delay(5000, _tokenSource);
        await Console.Out.WriteAsync($"{firstNodeName}, {secondNodeName}, {numberOfHops}");
    }
}