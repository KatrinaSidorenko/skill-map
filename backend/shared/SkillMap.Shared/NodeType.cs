using System.Runtime.CompilerServices;

namespace SkillMap.Shared;

// todo: extract to domain -> domain to library
public static class NodeType
{
    public const string Roadmap = "roadmap";
    public const string Topic = "topic";
    public const string SubTopic = "subtopic";
    public const string Resource = "resource";

    public static bool IsTopic(this string type)
        => type == Topic;
    public static bool IsSubTopic(this string type)
        => type == SubTopic;

    public static bool IsRoadmap(this string type)
        => type == Roadmap;

    public static bool IsResource(this string type)
        => type == Resource;
}
