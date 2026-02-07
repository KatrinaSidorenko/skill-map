using LearningPlatform.Roadmap.Business.Contracts.Constants;
using LearningPlatform.Roadmap.Business.Contracts.Models;

namespace LearningPlatform.Roadmap.Business.Helpers;

public static class NodesValidator
{
    public static bool IsValid(this NodeDto node)
        => node != null && node.Title != null;

    public static bool HasRoadmapNode(this List<NodeDto> nodes)
        => nodes.Any(node => node.Type == NodeType.Roadmap);
    public static bool IsTopic(this NodeDto node)
        => node.Type == NodeType.Topic;
    public static bool IsSubTopic(this NodeDto node)
        => node.Type == NodeType.SubTopic;
}