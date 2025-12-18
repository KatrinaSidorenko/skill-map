using LearningPlatform.Roadmap.Business.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillMap.Business.RoadmapTest.Helpers;

public class RoadmapRebuilder
{
    public (List<MarkNode> NewNodes, List<Edge> NewEdges) RebuildRemainingRoadmap(
        List<MarkNode> nodes,
        List<Edge> edges)
    {
        // 1. Identify which nodes to KEEP (Active Work) and REMOVE (Done)
        var nodesToKeep = new HashSet<string>(
            nodes.Where(n => n.MarkType != NodeMarkType.Completed.ToString())
                 .Select(n => n.Id)
        );

        // We also need a fast lookup for existing edges
        // Group edges by Source and Target for fast navigation
        var edgesBySource = edges.ToLookup(e => e.Source);
        var edgesByTarget = edges.ToLookup(e => e.Target);

        var finalEdges = new List<Edge>();
        var processedConnections = new HashSet<string>(); // To avoid duplicate edges

        // 2. Build the New Edge List
        // We iterate through every ACTIVE node and look for its ACTIVE children.
        // If a child is NOT active (i.e., Completed), we look *through* it to find the next active descendant.

        foreach (var sourceId in nodesToKeep)
        {
            // Find all reachable active nodes starting from this source
            // BFS/DFS to skip over "Completed" nodes
            var reachableActiveTargets = FindNextActiveTargets(sourceId, nodesToKeep, edgesBySource);

            foreach (var targetId in reachableActiveTargets)
            {
                var connectionKey = $"{sourceId}-{targetId}";
                if (!processedConnections.Contains(connectionKey))
                {
                    finalEdges.Add(new Edge
                    {
                        Id = Guid.NewGuid().ToString(), // Generate new ID for remapped edge
                        Source = sourceId,
                        Target = targetId
                    });
                    processedConnections.Add(connectionKey);
                }
            }
        }

        // 3. Filter the Nodes List
        var finalNodes = nodes.Where(n => nodesToKeep.Contains(n.Id)).ToList();

        return (finalNodes, finalEdges);
    }

    /// <summary>
    /// Recursive search to find the nearest "Active" descendants.
    /// If an immediate child is "Completed", we keep digging until we hit an "Active" one.
    /// </summary>
    private List<string> FindNextActiveTargets(
        string currentId,
        HashSet<string> nodesToKeep,
        ILookup<string?, Edge> edgesBySource)
    {
        var result = new List<string>();
        var directEdges = edgesBySource[currentId];

        foreach (var edge in directEdges)
        {
            var targetId = edge.Target;
            if (targetId == null) continue;

            if (nodesToKeep.Contains(targetId))
            {
                // Found a direct active child. Stop digging on this path.
                result.Add(targetId);
            }
            else
            {
                // The child is COMPLETED (Hidden).
                // "Bridge" the gap: Dig deeper to find what comes AFTER this completed node.
                result.AddRange(FindNextActiveTargets(targetId, nodesToKeep, edgesBySource));
            }
        }

        return result;
    }
}
