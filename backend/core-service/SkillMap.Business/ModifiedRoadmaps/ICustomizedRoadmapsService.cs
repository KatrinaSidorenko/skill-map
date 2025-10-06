using SkillMap.Business.ModifiedRoadmaps.Models;
using SkillMap.Business.Roadmaps.Models;
using SkillMap.Shared.Models;
using SkillMap.Shared.Results;

namespace SkillMap.Business.Roadmaps;

public interface ICustomizedRoadmapsService
{
    Task<Result<bool>> CreateLearningItem(long userId, string roadmapId, LearningItem learningItem, CancellationToken ct);
    Task<Result<bool>> CreateLearningItemsConnection(long userId, string roadmapId, LearningItemConnection connection, CancellationToken ct);
    Task<Result<bool>> DeleteRoadmap(long userId, string roadmapId, string itemId, CancellationToken ct);
    Task<Result<PaginationResult<List<PlainRoadmapWithDetailsDto>>>> GetPlainRoadmapsWithUserMetadata(long userId, SearchingParams @params, CancellationToken ct);
    Task<Result<SavedUerRoadmap>> GetRoadmap(long userId, string roadmapId, CancellationToken ct);
    Task<Result<bool>> SaveDeleteItemChange(long userId, string roadmapId, DeleteLearningItemChange itemChange, CancellationToken ct);
    Task<Result<bool>> SaveLearningItemChange(long userId, string roadmapId, LearningItemChange item, CancellationToken ct);
    Task<Result<bool>> UpdateStatus(long userId, string roadmapId, string itemId, UpdateStatusMetadata metadata, CancellationToken ct);
}
