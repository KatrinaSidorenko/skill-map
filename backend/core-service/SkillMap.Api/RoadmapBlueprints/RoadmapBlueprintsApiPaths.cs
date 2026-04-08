using LearningPlatform.Shared.Api;

namespace SkillMap.Api.RoadmapBlueprints;

internal static class RoadmapBlueprintsApiPaths
{
    private const string RoadmapBlueprintsRootApi = $"{ApiPaths.Root}/roadmap-blueprints";

    public const string GetRoadmapBlueprintsSummary = $"{RoadmapBlueprintsRootApi}";
    public const string GetRoadmapBlueprint = $"{RoadmapBlueprintsRootApi}/{{id}}";
}