namespace SkillMap.Core.Constants;
public static class LearningItemType
{
    public const string Roadmap = "roadmap";
    public const string Topic = "topic";
    public const string SubTopic = "subtopic";
    public const string Resource = "resource";
    public static List<string> GetTypes()
    {
        return new List<string>
        {
            Topic,
            SubTopic,
        };
    }
}