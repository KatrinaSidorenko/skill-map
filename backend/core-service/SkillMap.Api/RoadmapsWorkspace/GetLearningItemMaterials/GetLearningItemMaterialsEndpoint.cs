using SkillMap.Api.Roadmaps;
using SkillMap.Business.PersonalizedRoadmaps;
using SkillMap.Business.RoadmapsWorkspace.Features.GetLearningItemMaterials;

namespace SkillMap.Api.RoadmapsWorkspace.GetLearningItemMaterials;

internal static class GetLearningItemMaterialsEndpoint
{
    internal static void MapGetLearningItemMaterials(this IEndpointRouteBuilder app) => app.MapGet(RoadmapsWorkspaceApiPaths.GetLearningItemMaterials, async (
        long userRoadmapId,
        string learningItemId,
        IRoadmapWorkspaceModule roadmapWorkspaceModule,
        CancellationToken cancellationToken) =>
    {
        var query = new GetLearningItemMaterialsQuery(userRoadmapId, learningItemId);
        var result = await roadmapWorkspaceModule.ExecuteCommandAsync(query, cancellationToken);
        return Results.Ok(GetLearningItemMaterialsResponse.Create(result));
    })
        .Produces<GetLearningItemMaterialsResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}
