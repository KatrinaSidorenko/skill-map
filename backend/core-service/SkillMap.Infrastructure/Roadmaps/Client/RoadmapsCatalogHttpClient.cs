using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SkillMap.Business.Roadmaps.Models;
using SkillMap.Infrastructure.Roadmaps.Models;
using SkillMap.Shared.Results;

namespace SkillMap.Infrastructure.Roadmaps.Client;

public class RoadmapsCatalogHttpClient : IRoadmapsCatalogHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IRoadmapsCatalogHttpClient> _logger;
    public RoadmapsCatalogHttpClient(HttpClient httpClient, ILogger<IRoadmapsCatalogHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Result<List<Roadmap>>> GetAllRoadmaps(CancellationToken ct)
    {
        try
        {
            var response = await _httpClient.GetAsync("api/roadmaps", ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get roadmaps from the catalog. Status code: {StatusCode}", response.StatusCode);
                return ResultType.FailedToGetRoadmaps<List<Roadmap>>();
            }

            var content = await response.Content.ReadAsStringAsync(ct);
            var roadmaps = JsonConvert.DeserializeObject<RoadmapsResponse>(content);

            return Result.Success(roadmaps.Roadmaps);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting roadmaps from the catalog.");
            return ResultType.FailedToGetRoadmaps<List<Roadmap>>();
        }
    }

    public async Task<Result<TreeResponse>> GetFullRoadmap(string roadmapId, CancellationToken ct)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/roadmaps/{roadmapId}/graph", ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get the full roadmap from the catalog. Status code: {StatusCode}", response.StatusCode);
                return ResultType.FailedToGetRoadmap<TreeResponse>(roadmapId);
            }

            var content = await response.Content.ReadAsStringAsync(ct);
            var roadmap = JsonConvert.DeserializeObject<TreeResponse>(content);

            if (roadmap == null)
            {
                _logger.LogError("Failed to deserialize the full roadmap response.");
                return ResultType.FailedToGetRoadmap<TreeResponse>(roadmapId);
            }

            return Result.Success(roadmap);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting the full roadmap from the catalog.");
            return ResultType.FailedToGetRoadmap<TreeResponse>(roadmapId);
        }
    }

    public async Task<Result<TreePlainResponse>> GetFullPlainRoadmap(string roadmapId, CancellationToken ct)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/roadmaps/{roadmapId}", ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get the full roadmap from the catalog. Status code: {StatusCode}", response.StatusCode);
                return ResultType.FailedToGetRoadmap<TreePlainResponse>(roadmapId);
            }

            var content = await response.Content.ReadAsStringAsync(ct);
            var roadmap = JsonConvert.DeserializeObject<TreePlainResponse>(content);

            if (roadmap == null)
            {
                _logger.LogError("Failed to deserialize the full roadmap response.");
                return ResultType.FailedToGetRoadmap<TreePlainResponse>(roadmapId);
            }

            return Result.Success(roadmap);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting the full roadmap from the catalog.");
            return ResultType.FailedToGetRoadmap<TreePlainResponse>(roadmapId);
        }
    }
}
