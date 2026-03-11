using SkillMap.Business.RoadmapBlueprints;
using SkillMap.Business.RoadmapBlueprints.GetRoadmapBlueprint;

namespace SkillMap.Api.RoadmapBlueprints.GetRoadmapBlueprint;

internal static class GetRoadmapBlueprintEndpoint
{
    internal static void MapGetRoadmapBlueprint(this IEndpointRouteBuilder app) => app.MapGet(RoadmapBlueprintsApiPaths.GetRoadmapBlueprint, async (
            string id,
            IRoadmapBlueprintModule roadmapBlueprintModule,
  CancellationToken cancellationToken) =>
    {
        var query = new GetRoadmapBlueprintQuery(id);
        var result = await roadmapBlueprintModule.ExecuteCommandAsync(query, cancellationToken);
        return Results.Ok(GetRoadmapBlueprintResponse.Create(result));
  })
        .Produces<GetRoadmapBlueprintResponse>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}
