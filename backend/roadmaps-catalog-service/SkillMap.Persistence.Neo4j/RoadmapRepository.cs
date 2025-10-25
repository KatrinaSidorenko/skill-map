using LearningPlatform.Roadmap.Business.Contracts;
using LearningPlatform.Roadmap.Business.Contracts.Constants;
using LearningPlatform.Roadmap.Business.Contracts.Models;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using SkillMap.Persistence.Neo4j.Helpers;
using SkillMap.Shared.Models;
using SkillMap.Shared.Results;

namespace SkillMap.Persistence.Neo4j;

internal class RoadmapRepository : BaseRepository, IRoadmapRepository
{
    public RoadmapRepository(
        IDriver driver,
        DbSettings dbSettings,
        ILogger<IRoadmapRepository> logger) : base(driver, dbSettings, logger) { }

    public async Task<Result<bool>> CreateNodes(List<NodeDto> nodes, CancellationToken ct = default)
    {
        var newNodes = nodes.Select(n => n.GenerateInnerId()).ToList();
        var nodeCreateCommands = newNodes
           .Select(n => n.CreateNodeCommand())
           .ToList();

        return await ExecuteCommands(nodeCreateCommands, ct);
    }
    public async Task<Result<bool>> CreateEdges(List<EdgeDto> edges, CancellationToken ct = default)
    {
        var newEdges = edges.Select(e => e.GenerateInnerId()).ToList();
        var edgeCreateCommands = newEdges
            .Select(e => e.CreateEdgeCommand())
            .ToList();

        return await ExecuteCommands(edgeCreateCommands, ct);
    }
    public async Task<Result<bool>> UpdateNodes(List<NodeDto> nodes, CancellationToken ct = default)
    {
        var nodeUpdateCommands = nodes
            .Select(n => n.UpdateNodeCommand())
            .ToList();
        return await ExecuteCommands(nodeUpdateCommands, ct);
    }

    public async Task<Result<(List<Dictionary<string, object>> Nodes, List<Dictionary<string, object>> Edges)>>
        GetSourceRoadmap(string roadmapId, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        try
        {
            var query = @"
                MATCH (r:ROADMAP {id: $id})
                OPTIONAL MATCH path = (r)-[*0..]->(connected)
                RETURN r, path, connected";

            var (response, _, _) = await Driver.ExecutableQuery(query)
                .WithConfig(new QueryConfig(database: DbSettings.Name))
                .WithParameters(new { id = roadmapId })
                .ExecuteAsync();

            var nodesSet = new Dictionary<string, Dictionary<string, object>>();
            var edgesSet = new Dictionary<string, Dictionary<string, object>>();

            foreach (var record in response)
            {
                var rNode = record["r"]?.As<INode>();
                if (rNode != null && !nodesSet.ContainsKey(rNode.ElementId))
                {
                    nodesSet[rNode.ElementId] = rNode.ToDict();
                }

                var connected = record["connected"]?.As<INode>();
                if (connected != null && !nodesSet.ContainsKey(connected.ElementId))
                {
                    nodesSet[connected.ElementId] = connected.ToDict();
                }

                if (record["path"] is IPath path)
                {
                    foreach (var node in path.Nodes)
                    {
                        if (!nodesSet.ContainsKey(node.ElementId))
                        {
                            nodesSet[node.ElementId] = node.ToDict();
                        }
                    }

                    foreach (var rel in path.Relationships)
                    {
                        if (!edgesSet.ContainsKey(rel.ElementId))
                        {
                            edgesSet[rel.ElementId] = rel.ToDict(nodesSet);
                        }
                    }
                }
            }

            var separateNodesAndEdgesResult = await GetRoadmapSeparateNodesAndEdges(roadmapId, ct);
            var targetNodes = separateNodesAndEdgesResult.Nodes.Concat(nodesSet.Values).ToList();
            var targetEdges = separateNodesAndEdgesResult.Edges.Concat(edgesSet.Values).ToList();

            return Result.Success((targetNodes, targetEdges));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get roadmap {roadmapId}", roadmapId);
            return ResultType.FailedToGetRoadmap<(List<Dictionary<string, object>> Nodes, List<Dictionary<string, object>> Edges)>(ex.Message);
        }
    }

    private async Task<(List<Dictionary<string, object>> Nodes, List<Dictionary<string, object>> Edges)> GetRoadmapSeparateNodesAndEdges(string roadmapId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var query = @"
            MATCH (n)
            WHERE n.roadmap_id = $id
            OPTIONAL MATCH (n)-[r]->(m)
            WHERE m.roadmap_id = $id
            RETURN n, r, m";
        using var session = Driver.AsyncSession(s => s.WithDatabase(DbSettings.Name));
        var nodesSet = new Dictionary<string, Dictionary<string, object>>();
        var edgesSet = new Dictionary<string, Dictionary<string, object>>();

        await session.ExecuteReadAsync(async tx =>
        {
            var result = await tx.RunAsync(query, new { id = roadmapId });
            while (await result.FetchAsync())
            {
                var nNode = result.Current["n"]?.As<INode>();
                if (nNode != null && !nodesSet.ContainsKey(nNode.ElementId))
                {
                    nodesSet[nNode.ElementId] = nNode.ToDict();
                }
                var mNode = result.Current["m"]?.As<INode>();
                if (mNode != null && !nodesSet.ContainsKey(mNode.ElementId))
                {
                    nodesSet[mNode.ElementId] = mNode.ToDict();
                }
                var rRel = result.Current["r"]?.As<IRelationship>();
                if (rRel != null && !edgesSet.ContainsKey(rRel.ElementId))
                {
                    edgesSet[rRel.ElementId] = rRel.ToDict(nodesSet);
                }
            }
        });
        await session.CloseAsync();
        return (nodesSet.Values.ToList(), edgesSet.Values.ToList());
    }
    public async Task<Result<(List<NodeDto> Nodes, List<EdgeDto> Edges)>> GetRoadmapById(string roadmapId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var sourceRoadmapResult = await GetSourceRoadmap(roadmapId, ct);
        if (!sourceRoadmapResult.IsSuccessful)
        {
            return ResultType.FailedToGetRoadmap<(List<NodeDto> Nodes, List<EdgeDto> Edges)>(sourceRoadmapResult.Message);
        }

        var (nodes, edges) = sourceRoadmapResult.Data;
        var nodesDict = nodes.Select(n => n.ToNodeDto())
            .DistinctBy(n => n.Id)
            .ToDictionary(n => n.Id, n => n);
        var edgesList = edges.Select(e => e.ToEdgeDto(nodesDict))
            .GroupBy(e => (e.Source.Id, e.Target.Id))
            .Select(e => e.FirstOrDefault())
            .ToList();
        var nodesList = nodesDict.Values.ToList();

        return Result.Success((nodesList, edgesList));
    }

    public async Task<Result<PaginationResult<List<NodeDto>>>> GetPublicPlainRoadmapsByIds(
        List<string> roadmapIds,
        SearchingParams @params,
        CancellationToken ct,
        bool excludePrivate = true)
    {
        ct.ThrowIfCancellationRequested();

        try
        {
            using var session = Driver.AsyncSession(s => s.WithDatabase(DbSettings.Name));

            var conditions = new List<string>() { };
            if (roadmapIds != null && roadmapIds.Count > 0)
            {
                conditions.Add("r.id IN $ids");
            }
            conditions.Add(CommandsBuilder.GetSearchCase());

            if (excludePrivate)
            {
                conditions.Add(CommandsBuilder.GetExcludePrivateRoadmapCondition());
            }

            var whereClause = string.Join(" AND ", conditions);

            // --- Query for page of data ---
            var query = $@"
                MATCH (r:{RoadmapLabelConstants.ROADMAP})
                WHERE {whereClause}
                RETURN r
                SKIP $skip
                LIMIT $limit";

            var response = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync(query, new
                {
                    ids = roadmapIds,
                    skip = @params.PaginationParams.Skip,
                    limit = @params.PaginationParams.PageSize,
                    searchTerm = @params.SearchTermByName
                });

                var nodes = new List<NodeDto>();
                while (await result.FetchAsync())
                {
                    var node = result.Current["r"].As<INode>().Properties.ToDictionary().ToNodeDto();
                    nodes.Add(node);
                }

                return nodes;
            });

            // --- Count query ---
            var countQuery = $@"
                MATCH (r:{RoadmapLabelConstants.ROADMAP})
                WHERE {whereClause}
                RETURN count(r) AS total";

            var totalCount = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync(countQuery, new
                {
                    ids = roadmapIds,
                    searchTerm = @params.SearchTermByName
                });
                await result.FetchAsync();
                return result.Current["total"].As<int>();
            });

            await session.CloseAsync();

            return Result.Success(new PaginationResult<List<NodeDto>>
            {
                Result = response,
                TotalCount = totalCount
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get roadmaps by ids");
            return ResultType.FailedToGetRoadmap<PaginationResult<List<NodeDto>>>(ex.Message);
        }
    }

    // todo: get tota count by the ids of nodes
    public async Task<Result<Dictionary<string, int>>> CalculateTotalTopicsAndSubtopics(List<string> roadmapIds, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            using var session = Driver.AsyncSession(s => s.WithDatabase(DbSettings.Name));

            var query = @"
                UNWIND $ids AS roadmapId
OPTIONAL MATCH (r:ROADMAP {id: roadmapId})
OPTIONAL MATCH (r)-[:CONTAINS|LEADS_TO|HAS_SUBTOPIC*1..]->(n)
WHERE n:TOPIC OR n:SUBTOPIC
RETURN roadmapId, count(DISTINCT n) AS totalTopicsAndSubtopics;
";
            var totalTopics = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync(query, new { ids = roadmapIds });
                var topicsCountDict = new Dictionary<string, int>();
                while (await result.FetchAsync())
                {
                    var id = result.Current["roadmapId"].As<string>();
                    var count = result.Current["totalTopicsAndSubtopics"].As<int>();
                    topicsCountDict[id] = count;
                }
                return topicsCountDict;
            });

            // topic that are floating and has the roadmap_id property // mostly for roadmaps created by users 
            var floatingQuery = @"
                UNWIND $ids AS roadmapId
                MATCH (n)
                WHERE n.roadmap_id = roadmapId AND (n:TOPIC OR n:SUBTOPIC)
                RETURN roadmapId, count(DISTINCT n) AS totalFloatingTopicsAndSubtopics;";
            var floatingTopics = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync(floatingQuery, new { ids = roadmapIds });
                var topicsCountDict = new Dictionary<string, int>();
                while (await result.FetchAsync())
                {
                    var id = result.Current["roadmapId"].As<string>();
                    var count = result.Current["totalFloatingTopicsAndSubtopics"].As<int>();
                    topicsCountDict[id] = count;
                }
                return topicsCountDict;
            });

            foreach (var kvp in floatingTopics ?? [])
            {
                if (totalTopics.ContainsKey(kvp.Key))
                {
                    totalTopics[kvp.Key] += kvp.Value;
                }
                else
                {
                    totalTopics[kvp.Key] = kvp.Value;
                }
            }

            await session.CloseAsync();
            return Result.Success(totalTopics);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to calculate total topics and subtopics for roadmaps");
            return ResultType.FailedToGetRoadmap<Dictionary<string, int>>(ex.Message);
        }
    }

    public async Task<Result<List<ResourceDto>>> GetRoadmapItemMaterials(
        string roadmapId,
        string itemId,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        try
        {
            using var session = Driver.AsyncSession(s => s.WithDatabase(DbSettings.Name));

            var query = $@"
    MATCH (item {{id: $itemId}})
    OPTIONAL MATCH (item)-[:{RoadmapLabelConstants.HAS_RESOURCE}]->(material:{RoadmapLabelConstants.RESOURCE})
    RETURN material";

            var response = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync(query, new { itemId });
                var materials = new List<ResourceDto>();

                while (await result.FetchAsync())
                {
                    var node = result.Current["material"]?.As<INode>();
                    if (node != null)
                    {
                        var material = node.Properties.ToDictionary().ToResourceDto();
                        materials.Add(material);
                    }
                }

                return materials;
            });

            await session.CloseAsync();
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            Logger.LogError(
                ex,
                "Failed to get materials for item {itemId} in roadmap {roadmapId}",
                itemId,
                roadmapId
            );
            return ResultType.FailedToGetRoadmap<List<ResourceDto>>(ex.Message);
        }
    }

    public async Task<Result<bool>> RoadmapExists(string roadmapId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            using var session = Driver.AsyncSession(s => s.WithDatabase(DbSettings.Name));
            var query = $@"
                MATCH (r:{RoadmapLabelConstants.ROADMAP} {{id: $id}})
                RETURN count(r) AS roadmapCount";
            var exists = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync(query, new { id = roadmapId });
                await result.FetchAsync();
                var count = result.Current["roadmapCount"].As<int>();
                return count > 0;
            });
            await session.CloseAsync();
            return Result.Success(exists);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to check if roadmap {roadmapId} exists", roadmapId);
            return ResultType.FailedToGetRoadmap<bool>(ex.Message);
        }
    }
    public async Task<Result<List<NodeDto>>> GetNodesByIds(List<string> ids, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            using var session = Driver.AsyncSession(s => s.WithDatabase(DbSettings.Name));
            var query = $@"
                MATCH (n)
                WHERE n.id IN $ids
                RETURN n";
            var response = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync(query, new { ids });
                var nodes = new List<NodeDto>();
                while (await result.FetchAsync())
                {
                    var node = result.Current["n"]?.As<INode>();
                    if (node != null)
                    {
                        var dto = node.Properties.ToDictionary().ToNodeDto();
                        nodes.Add(dto);
                    }
                }
                return nodes;
            });
            await session.CloseAsync();
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            Logger.LogError(
                ex,
                "Failed to get nodes for ids: {ids}",
                string.Join(", ", ids)
            );
            return ResultType.FailedToGetRoadmap<List<NodeDto>>(ex.Message);
        }
    }

    public async Task<Result<bool>> DeleteRoadmapElement(string roadmapId, string elementId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            using var session = Driver.AsyncSession(s => s.WithDatabase(DbSettings.Name));
            var query = @"
                MATCH (n)
                WHERE n.roadmap_id = $roadmapId AND n.id = $elementId
                DETACH DELETE n";
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { roadmapId, elementId });
            });
            await session.CloseAsync();
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to delete roadmap element {elementId} from roadmap {roadmapId}", elementId, roadmapId);
            return ResultType.FailedToGetRoadmap<bool>(ex.Message);
        }
    }

    public async Task<Result<bool>> DeleteRoadmap(string roadmapId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            using var session = Driver.AsyncSession(s => s.WithDatabase(DbSettings.Name));
            var query = @"
                MATCH (n)
                WHERE n.roadmap_id = $roadmapId OR (n:ROADMAP AND n.id = $roadmapId)
                DETACH DELETE n";
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { roadmapId });
            });
            await session.CloseAsync();
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to delete roadmap {roadmapId}", roadmapId);
            return ResultType.FailedToGetRoadmap<bool>(ex.Message);
        }
    }
}
