using SkillMap.DataSource.RoadmapSh.Api;
using SkillMap.Shared.Models;

namespace SkillMap.DataSource.RoadmapSh.Mappers;

public static class NodeMapper
{
    public static string MapToCoreNodeType(this string type) => type switch
    {
        "topic" => "topic",
        "button" => "topic",
        "subtopic" => "subtopic",
        "paragraph" => "subtopic",
        "label" => "subtopic",
        "roadmap" => "roadmap",
        _ => "Unknown"
    };

    public static bool IsValidNode(this NodeDto node) => node != null && !node.IsUnknown() && node.ExternalId != null;
    public static bool IsUnknown(this NodeDto node) => node.Type == "Unknown";

    public static NodeDto MapToNodeDto(this RoadmapNode node)
    {
        return new NodeDto
        {
            ExternalId = node.Id,
            Title = node?.Data?.Label ?? "",
            //Description = node.Description,
            Type = node?.Type.MapToCoreNodeType(),
        };
    }
}
