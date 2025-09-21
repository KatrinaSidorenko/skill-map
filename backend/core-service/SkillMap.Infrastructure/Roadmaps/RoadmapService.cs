using SkillMap.Business.Abstractions;
using SkillMap.Business.Roadmaps.Models;
using SkillMap.Infrastructure.Roadmaps.Client;
using SkillMap.Shared.Results;

namespace SkillMap.Infrastructure.Roadmaps;

public class RoadmapService(IRoadmapsCatalogHttpClient catalogHttpClient) : IRoadmapService
{
    public async Task<Result<List<RoadmapDto>>> GetRoadmaps(List<string> roadmapIds, CancellationToken ct)
    {
        // todo: add logic to get roadmaps by ids
        var response = await catalogHttpClient.GetAllRoadmaps(ct);
        if (!response.IsSuccessful)
        {
            return response;
        }

        var roadmaps = response.Data
            .Where(r => roadmapIds.Contains(r.Id))
            .ToList();

        return Result.Success(roadmaps);
    }

    public async Task<Result<TreeResponse>> GetFullRoadmap(string roadmapId, CancellationToken ct)
    {
        var response = await catalogHttpClient.GetFullRoadmap(roadmapId, ct);
        if (!response.IsSuccessful)
        {
            return response;
        }

        return Result.Success(response.Data);
    }

    public async Task<Result<TreePlainResponse>> GetFullPlainRoadmap(string roadmapId, CancellationToken ct)
    {
        var response = await catalogHttpClient.GetFullPlainRoadmap(roadmapId, ct);
        if (!response.IsSuccessful)
        {
            return response;
        }

        return Result.Success(response.Data);
    }
}
