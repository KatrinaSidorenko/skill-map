using LearningPlatform.Roadmap.Business.Contracts.Models;
using LearningPlatform.Roadmap.Business.Contracts.Constants;
using SkillMap.Shared.Extensions;

namespace SkillMap.Persistence.Neo4j.Helpers;

public static class CommandsBuilder
{
    public static Command CreateNodeCommand(this NodeDto node, string migrationId = null)
    {
        var nodeProps = new Dictionary<string, object>
        {
           // {"id", node.Id},
            {"title", node.Title},
            {"externalId", node.ExternalId},
            {"type", node.Type},
            {"migrationId", migrationId ?? string.Empty}
        };

        if (node.AdditionalProps != null)
        {
            foreach (var prop in node.AdditionalProps)
            {
                nodeProps[prop.Key] = prop.Value;
            }
        }

        var label = node.GetLabel();

        var createNodeQuery = @$"
    MERGE (n:{label} {{externalId: $externalId}})
    SET n += $props,
        n.id = coalesce(n.id, $id)
";


        return new Command
        {
            Text = createNodeQuery,
            Value = new { externalId = node.ExternalId, props = nodeProps, id = node.Id }
        };

    }

    public static Command CreateEdgeCommand(this EdgeDto edge, Dictionary<string, NodeDto> nodesByExId, string migrationId = null)
    {
        var edgeProps = new Dictionary<string, object>
        {
            {"id", edge.Id},
            {"migrationId", migrationId ?? string.Empty}
            //{"type", edge.Type}
        };

        var createEdgeQuery = @$"
            MATCH (source:{edge.Source.GetLabel()} {{externalId: $sourceInnerId}})
            MATCH (target:{edge.Target.GetLabel()} {{externalId: $targetInnerId}})
            MERGE (source)-[r:{edge.GetLabel()}]->(target)
            SET r += $props
        ";

        var sourceNode = nodesByExId.GetOrDefault(edge.Source.ExternalId);
        var targetNode = nodesByExId.GetOrDefault(edge.Target.ExternalId);

        if (sourceNode == null || targetNode == null)
        {
            sourceNode = edge.Source;
            //targetNode = edge.Target;
            //throw new ArgumentException($"Source or target node not found for edge: {edge.Id}");
        }

        return new Command
        {
            Text = createEdgeQuery,
            Value = new { sourceInnerId = sourceNode.ExternalId, targetInnerId = targetNode.ExternalId, props = edgeProps }
        };
    }

    public static Command CreateEdgeCommand(this EdgeDto edge, string migrationId = null)
    {
        var edgeProps = new Dictionary<string, object>
        {
            {"id", edge.Id},
            {"migrationId", migrationId ?? string.Empty}
            //{"type", edge.Type}
        };

        var createEdgeQuery = @$"
            MATCH (source:{edge.Source.GetLabel()} {{externalId: $sourceInnerId}})
            MATCH (target:{edge.Target.GetLabel()} {{externalId: $targetInnerId}})
            MERGE (source)-[r:{edge.GetLabel()}]->(target)
            SET r += $props
        ";

       
        return new Command
        {
            Text = createEdgeQuery,
            Value = new { sourceInnerId = edge.Source.ExternalId, targetInnerId = edge.Target.ExternalId, props = edgeProps }
        };
    }
}
