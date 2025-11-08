using LearningPlatform.Roadmap.Business.Contracts;
using LearningPlatform.Roadmap.Business.Contracts.Constants;
using LearningPlatform.Roadmap.Business.Contracts.Models;
using LearningPlatform.Roadmap.Business.Mappers;
using Microsoft.Extensions.Logging;
using Serilog;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Models;
using SkillMap.Shared.Results;

namespace LearningPlatform.Roadmap.Business;

public class RoadmapService(
    IRoadmapRepository roadmapRepository, 
    ILogger<IRoadmapService> logger) : IRoadmapService
{
    public async Task<Result<PaginationResult<List<PlainRoadmapDto>>>> GetPlainRoadmaps(SearchingParams @params, CancellationToken ct)
    {
        var paginatedResult = await roadmapRepository.GetPublicPlainRoadmapsByIds([], @params, ct);
        if (!paginatedResult.IsSuccessful)
        {
            Log.Error("Failed to get plain roadmaps: {Error}", paginatedResult.Message);
            return ResultType.FailedToGetRoadmaps<PaginationResult<List<PlainRoadmapDto>>>();
        }

        return Result.Success(new PaginationResult<List<PlainRoadmapDto>>
        {
            Result = paginatedResult.Data.Result?.ToPlainRoadmaps() ?? [],
            TotalCount = paginatedResult.Data.TotalCount
        });
    }
    public async Task<Result<RoadmapDto>> GetRoadmapById(string roadmapId, CancellationToken ct, bool includeStartNode = false)
    {
        var roadmapResult = await roadmapRepository.GetRoadmapById(roadmapId, ct);
        if (!roadmapResult.IsSuccessful)
        {
            Log.Error("Failed to get roadmap with ID {RoadmapId}: {Error}", roadmapId, roadmapResult.Message);
            return ResultType.FailedToGetRoadmap<RoadmapDto>(roadmapResult.Message);
        }

        var (nodes, edges) = roadmapResult.Data;
        if (nodes == null || !nodes?.Any() == true)
        {
            Log.Error("Roadmap with ID {RoadmapId} does not exist or has no nodes", roadmapId);
            return ResultType.RoadmapNotFound<RoadmapDto>(roadmapId);
        }

        var startNode = nodes.FirstOrDefault(n => n.Type == NodeType.Roadmap);
        if (startNode == null)
        {
            Log.Error("Roadmap with ID {RoadmapId} does not have a start node", roadmapId);
            return ResultType.RoadmapNotFound<RoadmapDto>(roadmapId);
        }

        var topicsAndSubTopics = nodes.Where(n => n.Type == NodeType.Topic || n.Type == NodeType.SubTopic).ToList();
        if (includeStartNode)
        {
            topicsAndSubTopics.Add(startNode);
        }
        var targetEdges = edges.Where(e => topicsAndSubTopics.Any(n => n.Id == e.Target?.Id)).ToList();

        return Result.Success(new RoadmapDto
        {
            Id = startNode.Id,
            Title = startNode.Title,
            Description = startNode.Description,
            Nodes = topicsAndSubTopics.ToNodes(),
            Edges = targetEdges.ToEdges()
        });
    }
    public async Task<Result<PaginationResult<List<PlainRoadmapDto>>>> GetPlainRoadmapsByIds(List<string> roadmapIds, SearchingParams @params, CancellationToken ct, bool excludePrivate = true)
    {
        if ((roadmapIds ?? []).Count <= 0)
        {
            return Result.Success(new PaginationResult<List<PlainRoadmapDto>>
            {
                TotalCount = 0,
                Result = [],
            });
        }
        var paginatedResult = await roadmapRepository.GetPublicPlainRoadmapsByIds(roadmapIds, @params, ct, excludePrivate);
        if (!paginatedResult.IsSuccessful)
        {
            Log.Error("Failed to get plain roadmaps by IDs: {Error}", paginatedResult.Message);
            return ResultType.FailedToGetRoadmaps<PaginationResult<List<PlainRoadmapDto>>>();
        }

        var plainRoadmaps = paginatedResult.Data.Result?.ToPlainRoadmaps() ?? [];
        var topicsCountDict = await roadmapRepository.CalculateTotalTopicsAndSubtopics(plainRoadmaps.Select(r => r.Id).ToHashSet().ToList(), ct);
        foreach (var roadmap in plainRoadmaps)
        {
            var count = topicsCountDict.Data.GetOrDefault(roadmap.Id);
            roadmap.TotalTopics = count;
        }

        return Result.Success(new PaginationResult<List<PlainRoadmapDto>>
        {
            Result = plainRoadmaps,
            TotalCount = paginatedResult.Data.TotalCount
        });
    }
    public async Task<Result<List<ResourceDto>>> GetLearningItemMaterials(string roadmapId, string itemId, CancellationToken ct)
    {
        var resourcesResult = await roadmapRepository.GetRoadmapItemMaterials(roadmapId, itemId, ct);
        if (!resourcesResult.IsSuccessful)
        {
            Log.Error("Failed to get learning item resources for roadmap ID {RoadmapId} and item ID {ItemId}: {Error}", roadmapId, itemId, resourcesResult.Message);
            return ResultType.FailedToGet<List<ResourceDto>>(resourcesResult.Message);
        }
        return Result.Success(resourcesResult.Data);
    }
    public async Task CreateNode(string roadmapId, NodeDto node, CancellationToken ct = default)
    {
        if (node == null)
        {
            throw new ArgumentNullException(nameof(node));
        }

        var roadmapExistsResult = await roadmapRepository.RoadmapExists(roadmapId, ct);
        if (!roadmapExistsResult.IsSuccessful)
        {
            var errorMessage = $"Failed to verify existence of roadmap with ID {roadmapId}: {roadmapExistsResult.Message}";
            logger.LogError(errorMessage);
            throw new Exception(errorMessage);
        }

        var result = await roadmapRepository.CreateNodes(new List<NodeDto> { node }, ct);
        if (!result.IsSuccessful)
        {
            var errorMessage = $"Failed to create node with ExternalId {node.ExternalId}: {result.Message}";
            logger.LogError(errorMessage);
            throw new Exception(errorMessage);
        }
    }
    public async Task CreateEdge(EdgeDto edge, CancellationToken ct = default)
    {
        if (edge == null)
        {
            throw new ArgumentNullException(nameof(edge));
        }

        var targetEdgeNodeIds = new List<string> { edge.Source?.Id, edge.Target?.Id };
        var targetNodesResult = await roadmapRepository.GetNodesByIds(targetEdgeNodeIds, ct);
        if (!targetNodesResult.IsSuccessful)
        {
            var errorMessage = $"Failed to verify existence of nodes for edge from {edge.Source?.ExternalId} to {edge.Target?.ExternalId}: {targetNodesResult.Message}";
            logger.LogError(errorMessage);
            throw new Exception(errorMessage);
        }

        var nodesDict = targetNodesResult.Data.ToDictionary(n => n.Id, n => n);
        edge.Source = nodesDict.GetOrDefault(edge.Source?.Id) ?? edge.Source;
        edge.Target = nodesDict.GetOrDefault(edge.Target?.Id) ?? edge.Target;

        var result = await roadmapRepository.CreateEdges(new List<EdgeDto> { edge }, ct);
        if (!result.IsSuccessful)
        {
            var errorMessage = $"Failed to create edge from {edge.Source?.ExternalId} to {edge.Target?.ExternalId}: {result.Message}";
            logger.LogError(errorMessage);
            throw new Exception(errorMessage);
        }
    }
    public async Task DeleteRoadmapElement(string roadmapId, string itemId, string deleteItemType, CancellationToken ct)
    {
        if (roadmapId == null)
        {
            throw new ArgumentNullException(nameof(roadmapId));
        }
        if (itemId == null)
        {
            throw new ArgumentNullException(nameof(itemId));
        }

        if (deleteItemType.ToLower().ToLower() == CommonHelpers.Edge)
        {
            var (sourceId, targetId) = itemId.GetConnectionPoints();
            if (string.IsNullOrEmpty(targetId) || string.IsNullOrEmpty(sourceId))
            {
                throw new ArgumentException("Invalid edge itemId format. Expected format: 'sourceId-targetId'");
            }

            var deleteEdgeResult = await roadmapRepository.DeleteEdge(sourceId, targetId, ct);
            if (!deleteEdgeResult.IsSuccessful)
            {
                var errorMessage = $"Failed to delete edge from {sourceId} to {targetId}: {deleteEdgeResult.Message}";
                logger.LogError(errorMessage);
                throw new Exception(errorMessage);
            }
            return;
        }

        var result = await roadmapRepository.DeleteRoadmapElement(roadmapId, itemId, ct);
        if (!result.IsSuccessful)
        {
            var errorMessage = $"Failed to delete roadmap element with ID {itemId} from roadmap with ID {roadmapId}: {result.Message}";
            logger.LogError(errorMessage);
            throw new Exception(errorMessage);
        }
    }
    public async Task<Result<string>> CreateRoadmap(PlainRoadmapDto roadmapDto, CancellationToken ct)
    {
        // todo: add validations
        var node = roadmapDto.ToNodeDto();
        await roadmapRepository.CreateNodes(new List<NodeDto> { node }, ct);
        return Result.Success(node.Id);
    }
    public async Task UpdateNode(NodeDto node, CancellationToken ct)
    {
        var id = node.Id;
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        var result = await roadmapRepository.UpdateNodes([node], ct);
        if (!result.IsSuccessful)
        {
            var errorMessage = $"Failed to update node with ID {id}: {result.Message}";
            logger.LogError(errorMessage);
            throw new Exception(errorMessage);
        }
    }
    public async Task DeleteRoadmap(string roadmapId, CancellationToken ct)
    {
        if (roadmapId == null)
        {
            throw new ArgumentNullException(nameof(roadmapId));
        }
        var result = await roadmapRepository.DeleteRoadmap(roadmapId, ct);
        if (!result.IsSuccessful)
        {
            var errorMessage = $"Failed to delete roadmap with ID {roadmapId}: {result.Message}";
            logger.LogError(errorMessage);
            throw new Exception(errorMessage);
        }
    }
}
