using LearningPlatform.Roadmap.Business.Contracts.Models;

namespace SkillMap.Business.RoadmapBlueprints.GetRoadmapBlueprint;

public record GetRoadmapBlueprintQuery(string RoadmapId, long UserId) : ICommand<RoadmapBlueprintDto>;
