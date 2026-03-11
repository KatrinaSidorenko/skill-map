using SkillMap.Api.RoadmapBlueprints.GetRoadmapBlueprint;
using SkillMap.Api.RoadmapBlueprints.GetRoadmapBlueprintsSummary;

namespace SkillMap.Api.RoadmapBlueprints;

internal static class RoadmapBlueprintsEndpoints
{
    internal static void MapRoadmapBlueprints(this WebApplication app)
  {
        app.MapGetRoadmapBlueprintsSummary();
        app.MapGetRoadmapBlueprint();
 }
}
