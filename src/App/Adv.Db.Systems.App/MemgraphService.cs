using Neo4j.Driver;

namespace Adv.Db.Systems.App;

public class MemgraphService
{
    private static readonly string MemgraphUri = Environment.GetEnvironmentVariable("MEMGRAPH_URI") ?? "bolt://localhost:7687";
    private readonly IDriver _driver = GraphDatabase.Driver(MemgraphUri, AuthTokens.None);

    private readonly CancellationToken _tokenSource;

    public MemgraphService(CancellationToken tokenSource)
    {
        _tokenSource = tokenSource;
    }

    public async Task<TaskSummary> Task01(string nodeName)
    {
        var (records, summary) = await _driver
            .ExecutableQuery(Queries.ChildrenOfNode)
            .WithParameters(new { nodeName })
            .ExecuteAsync(_tokenSource)
            .RecordsAndSummary(_tokenSource);

        var querySummary = new QuerySummary(nameof(Task01), summary, records);
        var consoleOutput = $"sub-categories: [{string.Join(", ", records.Select(r => r["child"].As<INode>().Properties["name"]))}]";

        return new TaskSummary(querySummary, consoleOutput);
    }

    public async Task<TaskSummary> Task02(string nodeName)
    {
        var (records, summary) = await _driver
            .ExecutableQuery(Queries.ChildCount)
            .WithParameters(new { nodeName })
            .ExecuteAsync(_tokenSource)
            .RecordsAndSummary(_tokenSource);

        var querySummary = new QuerySummary(nameof(Task02), summary, records);
        var consoleOutput = $"children count: {records.Single()["childCount"].As<string>()}";

        return new TaskSummary(querySummary, consoleOutput);
    }

    public async Task<TaskSummary> Task03(string nodeName)
    {
        var (records, summary) = await _driver
            .ExecutableQuery(Queries.Grandchildren)
            .WithParameters(new { nodeName })
            .ExecuteAsync(_tokenSource)
            .RecordsAndSummary(_tokenSource);

        var querySummary = new QuerySummary(nameof(Task03), summary, records);
        var consoleOutput = $"grandchildren sub-categories: [{string.Join(", ", records.Select(r => r["grandchild"].As<INode>().Properties["name"]))}]";

        return new TaskSummary(querySummary, consoleOutput);
    }

    public async Task<TaskSummary> Task04(string nodeName)
    {
        var (records, summary) = await _driver
            .ExecutableQuery(Queries.Parents)
            .WithParameters(new { nodeName })
            .ExecuteAsync(_tokenSource)
            .RecordsAndSummary(_tokenSource);

        var querySummary = new QuerySummary(nameof(Task04), summary, records);
        var consoleOutput = $"parents: [{string.Join(", ", records.Select(r => r["parent"].As<INode>().Properties["name"]))}]";

        return new TaskSummary(querySummary, consoleOutput);
    }

    public async Task<TaskSummary> Task05(string nodeName)
    {
        var (records, summary) = await _driver
            .ExecutableQuery(Queries.ParentsCount)
            .WithParameters(new { nodeName })
            .ExecuteAsync(_tokenSource)
            .RecordsAndSummary(_tokenSource);

        var querySummary = new QuerySummary(nameof(Task05), summary, records);
        var consoleOutput = $"parents count: {records.Single()["parentCount"].As<string>()}";

        return new TaskSummary(querySummary, consoleOutput);
    }

    public async Task<TaskSummary> Task06(string nodeName)
    {
        var (records, summary) = await _driver
            .ExecutableQuery(Queries.Grandparents)
            .WithParameters(new { nodeName })
            .ExecuteAsync(_tokenSource)
            .RecordsAndSummary(_tokenSource);

        var querySummary = new QuerySummary(nameof(Task06), summary, records);
        var consoleOutput = $"grandparents: [{string.Join(", ", records.Select(r => r["grandparent"].As<INode>().Properties["name"]))}]";

        return new TaskSummary(querySummary, consoleOutput);
    }

    public async Task<TaskSummary> Task07()
    {
        var (records, summary) = await _driver
            .ExecutableQuery(Queries.UniqueName)
            .ExecuteAsync(_tokenSource)
            .RecordsAndSummary(_tokenSource);

        var querySummary = new QuerySummary(nameof(Task07), summary, records);
        var consoleOutput = $"unique names: {records.Single()["uniqueNameCount"].As<string>()}";

        return new TaskSummary(querySummary, consoleOutput);
    }

    public async Task<TaskSummary> Task08()
    {
        var (records, summary) = await _driver
            .ExecutableQuery(Queries.NotSubCategories)
            .ExecuteAsync(_tokenSource)
            .RecordsAndSummary(_tokenSource);

        var querySummary = new QuerySummary(nameof(Task08), summary, records);
        var consoleOutput = $"not sub-categories: [{string.Join(", ", records.Select(r => r["category"].As<INode>().Properties["name"]))}]";

        return new TaskSummary(querySummary, consoleOutput);
    }

    public async Task<TaskSummary> Task09()
    {
        var (records, summary) = await _driver
            .ExecutableQuery(Queries.NotSubCategoriesCount)
            .ExecuteAsync(_tokenSource)
            .RecordsAndSummary(_tokenSource);

        var querySummary = new QuerySummary(nameof(Task09), summary, records);
        var consoleOutput = $"not sub-categories count: {records.Single()["notSubCategoriesCount"].As<string>()}";

        return new TaskSummary(querySummary, consoleOutput);
    }

    public async Task<TaskSummary> Task10(int limit)
    {
        var (records, summary) = await _driver
            .ExecutableQuery(Queries.LargestNumberOfChildren)
            .WithParameters(new { limit })
            .ExecuteAsync(_tokenSource)
            .RecordsAndSummary(_tokenSource);

        var querySummary = new QuerySummary(nameof(Task10), summary, records);
        var consoleOutput = $"nodes with the largest number of children: [{string.Join(", ", records.Select(r => (
                r["parentWithMaxChildren"].As<INode>().Properties["name"],
                r["childCount"].As<int>())
            )
        )}]";

        return new TaskSummary(querySummary, consoleOutput);
    }

    public async Task<TaskSummary> Task11(int limit)
    {
        var (records, summary) = await _driver
            .ExecutableQuery(Queries.SmallestNumberOfChildren)
            .WithParameters(new { limit })
            .ExecuteAsync(_tokenSource)
            .RecordsAndSummary(_tokenSource);

        var querySummary = new QuerySummary(nameof(Task11), summary, records);
        var consoleOutput = $"nodes with the smallest number of children: [{string.Join(", ", records.Select(r => (
                r["parentWithMinChildren"].As<INode>().Properties["name"],
                r["childCount"].As<int>())
            )
        )}]";

        return new TaskSummary(querySummary, consoleOutput);
    }

    public async Task<TaskSummary> Task12(string oldNodeName, string newNodeName)
    {
        var (records, summary) = await _driver
            .ExecutableQuery(Queries.ChangeName)
            .WithParameters(new { oldNodeName, newNodeName })
            .ExecuteAsync(_tokenSource)
            .RecordsAndSummary(_tokenSource);

        var querySummary = new QuerySummary(nameof(Task12), summary, records);
        var consoleOutput = $"new category name: {records.Single()["category"].As<INode>().Properties["name"].As<string>()}";

        return new TaskSummary(querySummary, consoleOutput);
    }

    public async Task<TaskSummary> Task13(string nodeName, int newNodePopularity)
    {
        var (records, summary) = await _driver
            .ExecutableQuery(Queries.PopularityChange)
            .WithParameters(new { nodeName, newNodePopularity })
            .ExecuteAsync(_tokenSource)
            .RecordsAndSummary(_tokenSource);

        var record = records.Single();

        var querySummary = new QuerySummary(nameof(Task13), summary, records);
        var consoleOutput =
            $"[{record["n"].As<INode>().Properties["name"]}, " +
            $"{record["oldPopularityId"].As<string>()}, " +
            $"{record["newPopularity"].As<INode>().Properties["id"]}]";

        return new TaskSummary(querySummary, consoleOutput);
    }

    public async Task<TaskSummary> Task14(string firstNodeName, string secondNodeName, int numberOfHops)
    {
        var (records, summary) = await _driver
            .ExecutableQuery(Queries.AllPaths(numberOfHops))
            .WithParameters(new { firstNodeName, secondNodeName })
            .ExecuteAsync(_tokenSource)
            .RecordsAndSummary(_tokenSource);

        var querySummary = new QuerySummary(nameof(Task14), summary, records);
        var consoleOutput =
            $"paths: " +
            $"{string.Join(
                Environment.NewLine,
                records.Select(record => $"[{string.Join(", ", record["path"].As<IPath>().Nodes.Select(n => n.Properties["name"]))}]")
            )}";

        return new TaskSummary(querySummary, consoleOutput);
    }

    public async Task<TaskSummary> Task15(string firstNodeName, string secondNodeName, int numberOfHops)
    {
        var (records, summary) = await _driver
            .ExecutableQuery(Queries.AllPathsCount(numberOfHops))
            .WithParameters(new { firstNodeName, secondNodeName })
            .ExecuteAsync(_tokenSource)
            .RecordsAndSummary(_tokenSource);

        var querySummary = new QuerySummary(nameof(Task15), summary, records);
        var consoleOutput = $"count of different nodes existing on paths: {records.Single()["differentNodes"].As<string>()}";

        return new TaskSummary(querySummary, consoleOutput);
    }

    public async Task<TaskSummary> Task16(string nodeName, int radius)
    {
        var (records, summary) = await _driver
            .ExecutableQuery(Queries.NeighborhoodPopularity(radius))
            .WithParameters(new { nodeName })
            .ExecuteAsync(_tokenSource)
            .RecordsAndSummary(_tokenSource);

        var record = records.Single();

        var querySummary = new QuerySummary(nameof(Task16), summary, records);
        var consoleOutput =
            $"category name: {record["node_name"]}{Environment.NewLine}" +
            $"category popularity: {record["node_popularity"]}{Environment.NewLine}" +
            $"category neighbor count: {record["neighbor_count"]}{Environment.NewLine}" +
            $"popularity of neighboring categories: [{string.Join(", ", record["neighbor_popularity_tuples"].As<Dictionary<string, int>>())}]{Environment.NewLine}" +
            $"neighborhood popularity: {record["neighborhood_popularity"]}";

        return new TaskSummary(querySummary, consoleOutput);
    }

    public async Task<TaskSummary> Task17(string firstNodeName, string secondNodeName)
    {
        var (records, summary) = await _driver
            .ExecutableQuery(Queries.ShortestPathPopularity)
            .WithParameters(new { firstNodeName, secondNodeName })
            .ExecuteAsync(_tokenSource)
            .RecordsAndSummary(_tokenSource);

        var querySummary = new QuerySummary(nameof(Task17), summary, records);
        var consoleOutput =
            $"popularity on paths: {Environment.NewLine}" +
            $"{string.Join(Environment.NewLine, records.Select(record =>
                    $"{record["path_popularity"].As<string>()} [{string.Join(", ", record["path"].As<IPath>().Nodes.Select(n => n.Properties["name"]))}]"
                )
            )}";

        return new TaskSummary(querySummary, consoleOutput);
    }

    public async Task<TaskSummary> Task18(string firstNodeName, string secondNodeName, int numberOfHops, int limit)
    {
        var (records, summary) = await _driver.ExecutableQuery(Queries.DirectedPathWithHighestPopularity(numberOfHops))
            .WithParameters(new { firstNodeName, secondNodeName, limit })
            .ExecuteAsync(_tokenSource)
            .RecordsAndSummary(_tokenSource);

        var querySummary = new QuerySummary(nameof(Task18), summary, records);
        var consoleOutput =
            $"popularity on paths: {Environment.NewLine}" +
            $"{string.Join(Environment.NewLine, records.Select(record =>
                    $"{record["path_popularity"].As<string>()} [{string.Join(", ", record["path"].As<IPath>().Nodes.Select(n => n.Properties["name"]))}]"
                )
            )}";

        return new TaskSummary(querySummary, consoleOutput);
    }
}