namespace LearningPlatform.Roadmap.Business.Contracts.Constants;

public static class CommonHelpers
{
    public const string Node = "node";
    public const string Edge = "edge";

    public static (string SourceId, string TragetId) GetConnectionPoints(this string connection)
    {
        var parts = connection.Split("-");
        if (parts.Length != 2)
        {
            return (null, null);
        }

        return (parts[0], parts[1]);
    }
}