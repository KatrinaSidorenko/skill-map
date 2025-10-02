using LearningPlatform.Roadmap.Business.Contracts;
using LearningPlatform.Roadmap.Business.Contracts.Constants;
using LearningPlatform.Roadmap.Business.Contracts.Models;
using LearningPlatform.Roadmap.Business.Mappers;
using Microsoft.Extensions.Logging;
using Serilog;
using SkillMap.Api.Roadmaps.Models;
using SkillMap.Shared.Models;
using SkillMap.Shared.Results;

namespace LearningPlatform.Roadmap.Business;

public class RoadmapService(
    IRoadmapRepository roadmapRepository, 
    ILogger<IRoadmapService> logger) : IRoadmapService
{
    public async Task<Result<PaginationResult<List<PlainRoadmapDto>>>> GetPlainRoadmaps(PaginationParams paginationParams, CancellationToken ct)
    {
        var paginatedResult = await roadmapRepository.GetAllPlainRoadmaps(paginationParams, ct);
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

    public async Task<Result<RoadmapDto>> GetRoadmap(string roadmapId, CancellationToken ct)
    {
        var roadmapResult = await roadmapRepository.GetRoadmapById(roadmapId, ct);
        if (!roadmapResult.IsSuccessful)
        {
            Log.Error("Failed to get roadmap with ID {RoadmapId}: {Error}", roadmapId, roadmapResult.Message);
            return ResultType.FailedToGetRoadmap<RoadmapDto>(roadmapResult.Message);
        }

        var (nodes, edges) = roadmapResult.Data;
        var startNode = nodes.FirstOrDefault(n => n.Type == NodeType.Roadmap);
        if (startNode == null)
        {
            Log.Error("Roadmap with ID {RoadmapId} does not have a start node", roadmapId);
            return ResultType.RoadmapNotFound<RoadmapDto>(roadmapId);
        }

        return Result.Success(new RoadmapDto
        {
            Id = startNode.Id,
            Title = startNode.Title,
            Description = startNode.Description,
            Nodes = nodes.ToNodes(),
            Edges = edges.ToEdges()
        });
    }
}
