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
        ILogger<IRoadmapRepository> logger) : base(driver, dbSettings, logger) {}

    public async Task<bool> Save((List<NodeDto> Nodes, List<EdgeDto> Edges) graph, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        if (graph.Nodes == null || graph.Edges == null)
            throw new ArgumentNullException(nameof(graph));
        var migrationId = Guid.NewGuid().ToString("N");

        var nodes = graph.Nodes.Select(n => n.GenerateInnerId()).ToList();
        var nodesByExId = nodes
            .GroupBy(n => n.ExternalId ?? n.Id)
            .ToDictionary(n => n.Key, n => n.FirstOrDefault());
        var edges = graph.Edges.Select(e => e.GenerateInnerId()).ToList();

        var nodeCreateCommands = nodes
            .Select(n => n.CreateNodeCommand(migrationId))
            .ToList();

        var edgeCreateCommands = edges
            .Select(e => e.CreateEdgeCommand(nodesByExId, migrationId))
            .ToList();

        using var session = Driver.AsyncSession(s => s.WithDatabase(DbSettings.Name));
        using var transaction = await session.BeginTransactionAsync();

        try
        {
            await ExecuteCommands(transaction, nodeCreateCommands, ct);
            await ExecuteCommands(transaction, edgeCreateCommands, ct);
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw ex;
        }
        finally
        {
            await session.CloseAsync();
        }

        return true;
    }

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

            var nodesSet = new Dictionary<long, Dictionary<string, object>>();
            var edgesSet = new Dictionary<long, Dictionary<string, object>>();

            foreach (var record in response)
            {
                var rNode = record["r"]?.As<INode>();
                if (rNode != null && !nodesSet.ContainsKey(rNode.Id))
                {
                    nodesSet[rNode.Id] = rNode.ToDict();
                }

                var connected = record["connected"]?.As<INode>();
                if (connected != null && !nodesSet.ContainsKey(connected.Id))
                {
                    nodesSet[connected.Id] = connected.ToDict();
                }

                if (record["path"] is IPath path)
                {
                    foreach (var node in path.Nodes)
                    {
                        if (!nodesSet.ContainsKey(node.Id))
                        {
                            nodesSet[node.Id] = node.ToDict();
                        }
                    }

                    foreach (var rel in path.Relationships)
                    {
                        if (!edgesSet.ContainsKey(rel.Id))
                        {
                            edgesSet[rel.Id] = rel.ToDict(nodesSet);
                        }
                    }
                }
            }

            return Result.Success((nodesSet.Values.ToList(), edgesSet.Values.ToList()));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get roadmap {roadmapId}", roadmapId);
            return ResultType.FailedToGetRoadmap<(List<Dictionary<string, object>> Nodes, List<Dictionary<string, object>> Edges)>(ex.Message);
        }
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
            .ToDictionary(n => n.Id, n => n);
        var edgesList = edges.Select(e => e.ToEdgeDto(nodesDict)).ToList();
        var nodesList = nodesDict.Values.ToList();

        return Result.Success((nodesList, edgesList));
    }

    public async Task<Result<PaginationResult<List<NodeDto>>>> GetAllPlainRoadmaps(SearchingParams @params, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        try
        {
            using var session = Driver.AsyncSession(s => s.WithDatabase(DbSettings.Name));
            var query = @"
                MATCH (r:ROADMAP)
                WHERE 
                  CASE
                    WHEN $searchTerm IS NULL OR trim($searchTerm) = '' THEN true
                    ELSE toLower(r.title) CONTAINS toLower($searchTerm)
                  END
                RETURN r
                SKIP $skip
                LIMIT $limit";
            var response = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync(query, new
                {
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

            var countQuery = 
                @"MATCH (r:ROADMAP) 
                    WHERE 
                        CASE
                        WHEN $searchTerm IS NULL OR trim($searchTerm) = '' THEN true
                        ELSE toLower(r.title) CONTAINS toLower($searchTerm)
                        END
                RETURN count(r) as total";
            var totalCount = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync(countQuery, new
                {
                    searchTerm = @params.SearchTermByName
                });
                await result.FetchAsync();
                return result.Current["total"].As<int>();
            });

            await session.CloseAsync();
            
            return Result.Success(new PaginationResult<List<NodeDto>>
            {
                Result = response,
                TotalCount = totalCount,
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get all roadmaps");
            return ResultType.FailedToGetRoadmap<PaginationResult<List<NodeDto>>>(ex.Message);
        }
    }

    public async Task<Result<PaginationResult<List<NodeDto>>>> GetPlainRoadmapsByIds(List<string> roadmapIds, SearchingParams @params, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            using var session = Driver.AsyncSession(s => s.WithDatabase(DbSettings.Name));
            var query = @"
                MATCH (r:ROADMAP)
                WHERE r.id IN $ids
                AND (
                  CASE
                    WHEN $searchTerm IS NULL OR trim($searchTerm) = '' THEN true
                    ELSE toLower(r.title) CONTAINS toLower($searchTerm)
                  END
                )
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
            var countQuery = @"
                MATCH (r:ROADMAP) 
                WHERE r.id IN $ids 
                AND (
                  CASE
                    WHEN $searchTerm IS NULL OR trim($searchTerm) = '' THEN true
                    ELSE toLower(r.title) CONTAINS toLower($searchTerm)
                  END
                )
                RETURN count(r) as total";
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
                TotalCount = totalCount,
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get roadmaps by ids");
            return ResultType.FailedToGetRoadmap<PaginationResult<List<NodeDto>>>(ex.Message);
        }
    }

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

}
