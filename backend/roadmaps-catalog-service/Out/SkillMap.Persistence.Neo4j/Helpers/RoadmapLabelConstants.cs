using LearningPlatform.Roadmap.Business.Contracts.Constants;
using SkillMap.Application.OutPorts.Persistence;

namespace SkillMap.Persistence.Neo4j.Helpers;

public static class RoadmapLabelConstants
{
    public const string CONTAINS = "CONTAINS";
    public const string LEADS_TO = "LEADS_TO";
    public const string HAS_SUBTOPIC = "HAS_SUBTOPIC";
    public const string HAS_RESOURCE = "HAS_RESOURCE";

    public const string ROADMAP = "ROADMAP";
    public const string TOPIC = "TOPIC";
    public const string SUBTOPIC = "SUBTOPIC";
    public const string RESOURCE = "RESOURCE";

    public static string GetLabel(this NodeDao node)
        => node.Type switch
        {
            NodeType.Roadmap => ROADMAP,
            NodeType.Topic => TOPIC,
            NodeType.SubTopic => SUBTOPIC,
            NodeType.Resource => RESOURCE,
            _ => throw new ArgumentOutOfRangeException(nameof(node), $"Unknown node type: {node.Type}")
        };

    public static string GetLabel(this EdgeDao<NodeDao> edge)
    {
        if (edge.Source.Type.IsRoadmap() && edge.Target.Type.IsTopic())
        {
            return CONTAINS;
        }

        if (edge.Source.Type.IsTopic() && edge.Target.Type.IsTopic())
        {
            return LEADS_TO;
        }

        if (edge.Source.Type.IsTopic() && edge.Target.Type.IsSubTopic())
        {
            return HAS_SUBTOPIC;
        }

        if (edge.Source.Type.IsSubTopic() && edge.Target.Type.IsSubTopic())
        {
            return HAS_SUBTOPIC;
        }

        if (edge.Source.Type.IsSubTopic() && edge.Target.Type.IsResource())
        {
            return HAS_RESOURCE;
        }

        if (edge.Source.Type.IsTopic() && edge.Target.Type.IsResource())
        {
            return HAS_RESOURCE;
        }

        return LEADS_TO;
    }
}
