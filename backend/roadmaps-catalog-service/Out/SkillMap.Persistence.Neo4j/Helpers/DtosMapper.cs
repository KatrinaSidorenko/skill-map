using Neo4j.Driver;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Models;

namespace SkillMap.Persistence.Neo4j.Helpers;

public static class DtosMapper
{
    public static Dictionary<string, object> ToDict(this INode node)
    {
        var dict = node.Properties.ToDictionary();
        //dict["id"] = node.Id;
        dict["labels"] = node.Labels;
        return dict;
    }

    public static Dictionary<string, object> ToDict(this IRelationship rel,  Dictionary<long, Dictionary<string, object>> nodes)
    {
        var startId = rel.StartNodeId;
        var endId = rel.EndNodeId;
        var dict = rel.Properties.ToDictionary();
        dict["id"] = rel.Id;
        //dict["type"] = rel.Type;
        dict["source_id"] = nodes.GetOrDefault(startId).GetOrDefault("id");
        dict["target_id"] = nodes.GetOrDefault(endId).GetOrDefault("id");
        return dict;
    }

    public static NodeDto ToNodeDto(this Dictionary<string, object> dict)
    {
        var node = new NodeDto
        {
            Id = dict.GetOrDefault("id") as string,
            ExternalId = dict.GetOrDefault("externalId") as string,
            Title = dict.GetOrDefault("title") as string,
            Type = dict.GetOrDefault("type") as string,
            Description = dict.GetOrDefault("description") as string,
            AdditionalProps = new Dictionary<string, string>()
            {
                { "resource_link", dict.GetOrDefault("resource_link") as string },
                { "resource_type", dict.GetOrDefault("resource_type") as string },
            }
        };
        return node;
    }

    public static EdgeDto<NodeDto> ToEdgeDto(this Dictionary<string, object> dict, Dictionary<string, NodeDto> nodesDict)
    {
        var sourceId = dict.GetOrDefault("source_id") as string;
        var targetId = dict.GetOrDefault("target_id") as string;

        var edge = new EdgeDto<NodeDto>
        {
            Id = dict.GetOrDefault("id") as string,
            Source = nodesDict.GetOrDefault(sourceId),
            Target = nodesDict.GetOrDefault(targetId),
        };
        return edge;
    }

}
