namespace SkillMap.Business.RoadmapsWorkspace.Features.GetLearningItemMaterials;

public record GetLearningItemMaterialsQuery(long WorkspaceId, string LearningItemId) : ICommand<List<LearningItemMaterialDto>>;
