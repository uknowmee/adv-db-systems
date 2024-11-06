using Neo4j.Driver;

namespace Adv.Db.Systems.App;

public record TaskSummary(QuerySummary QuerySummary, string ConsoleOutput);

public record QuerySummary(string TaskName, IResultSummary Summary, IReadOnlyList<IRecord> Records);