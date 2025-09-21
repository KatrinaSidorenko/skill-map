using SkillMap.Business.Roadmaps.Models;
using SkillMap.Shared.Results;

namespace SkillMap.Business.Roadmaps;

public interface ICustomizedRoadmapsService
{
    Task<Result<NodeResponse>> Create(long userId, string roadmapId, CreateLearningItemMetadata itemMetadata, CancellationToken ct);
    Task<Result<bool>> DeleteRoadmap(long userId, string roadmapId, string itemId, CancellationToken ct);
    Task<Result<List<RoadmapDto>>> GetAllRoadmaps(long userId, CancellationToken ct);
    Task<Result<CustomizedUerRoadmap>> GetRoadmap(long userId, string roadmapId, CancellationToken ct);
    Task<Result<bool>> Update(long userId, string roadmapId, LearningItemSnapshot item, CancellationToken ct);
    Task<Result<bool>> UpdateStatus(long userId, string roadmapId, string itemId, UpdateStatusMetadata metadata, CancellationToken ct);
}
