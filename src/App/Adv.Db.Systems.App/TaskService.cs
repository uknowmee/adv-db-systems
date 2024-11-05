namespace Adv.Db.Systems.App;

public static class TaskService
{
    public static Task GetTask(this string[] args, CancellationToken tokenSource)
    {
        var memgraphService = new MemgraphService(tokenSource);

        return args switch
        {
            ["1", var nodeName] => memgraphService.Task01(nodeName),
            ["2", var nodeName] => memgraphService.Task02(nodeName),
            ["3", var nodeName] => memgraphService.Task03(nodeName),
            ["4", var nodeName] => memgraphService.Task04(nodeName),
            ["5", var nodeName] => memgraphService.Task05(nodeName),
            ["6", var nodeName] => memgraphService.Task06(nodeName),
            ["7"] => memgraphService.Task07(),
            ["8"] => memgraphService.Task08(),
            ["9"] => memgraphService.Task09(),
            ["10", var limit] => int.TryParse(limit, out var intLimit)
                ? memgraphService.Task10(intLimit)
                : throw new InvalidOperationException("Invalid arguments"),
            ["11", var limit] => int.TryParse(limit, out var intLimit)
                ? memgraphService.Task11(intLimit)
                : throw new InvalidOperationException("Invalid arguments"),
            ["12", var odlNodeName, var newNodeName] => memgraphService.Task12(odlNodeName, newNodeName),
            ["13", var nodeName, var newNodePopularity] => int.TryParse(newNodePopularity, out var intNewNodePopularity)
                ? memgraphService.Task13(nodeName, intNewNodePopularity)
                : throw new InvalidOperationException("Invalid arguments"),
            ["14", var firstNodeName, var secondNodeName, var numberOfHops] => int.TryParse(numberOfHops, out var intNumberOfHops)
                ? memgraphService.Task14(firstNodeName, secondNodeName, intNumberOfHops)
                : throw new InvalidOperationException("Invalid arguments"),
            ["15", var firstNodeName, var secondNodeName, var numberOfHops] => int.TryParse(numberOfHops, out var intNumberOfHops)
                ? memgraphService.Task15(firstNodeName, secondNodeName, intNumberOfHops)
                : throw new InvalidOperationException("Invalid arguments"),
            ["16", var nodeName, var radius] => int.TryParse(radius, out var intRadius)
                ? memgraphService.Task16(nodeName, intRadius)
                : throw new InvalidOperationException("Invalid arguments"),
            ["17", var firstNodeName, var secondNodeName] => memgraphService.Task17(firstNodeName, secondNodeName),
            ["18", var firstNodeName, var secondNodeName, var numberOfHops] => int.TryParse(numberOfHops, out var intNumberOfHops)
                ? memgraphService.Task18(firstNodeName, secondNodeName, intNumberOfHops)
                : throw new InvalidOperationException("Invalid arguments"),
            _ => throw new InvalidOperationException("Invalid arguments")
        };
    }
}