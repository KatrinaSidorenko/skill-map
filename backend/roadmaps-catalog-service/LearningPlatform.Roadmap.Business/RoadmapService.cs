using LearningPlatform.Roadmap.Business.Contracts;
using LearningPlatform.Roadmap.Business.Contracts.Models;
using LearningPlatform.Roadmap.Business.Mappers;
using Microsoft.Extensions.Logging;
using Serilog;
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
            Result = paginatedResult.Data.Result?.ToPlainRoadmaps(),
            TotalCount = paginatedResult.Data.TotalCount
        });
    }
}
