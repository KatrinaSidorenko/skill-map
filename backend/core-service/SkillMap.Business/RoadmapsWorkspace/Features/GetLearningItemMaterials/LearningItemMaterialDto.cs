using SkillMap.Core.RoadmapsWorkspace;

namespace SkillMap.Business.RoadmapsWorkspace.Features.GetLearningItemMaterials;

public record LearningItemMaterialDto(string Id, string Title, string Url, MaterialType Type);
