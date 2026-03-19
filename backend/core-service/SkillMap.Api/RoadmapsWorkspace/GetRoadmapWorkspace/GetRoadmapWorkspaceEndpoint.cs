using SkillMap.Api.Roadmaps;
using SkillMap.Business.PersonalizedRoadmaps;
using SkillMap.Business.PersonalizedRoadmaps.Features.GetPersonalizedRoadmap;

namespace SkillMap.Api.PersonalizedRoadmaps.GetPersonalizedRoadmap;

internal static class GetRoadmapWorkspaceEndpoint
{
    internal static void MapGetRoadmapWorkspace(this IEndpointRouteBuilder app) => app.MapGet(RoadmapsWorkspaceApiPaths.GetRoadmapWorkspace, async (
            long userRoadmapId,
            IRoadmapWorkspaceModule roadmapWorkspaceModule, CancellationToken cancellationToken) =>
    {
        var result = await roadmapWorkspaceModule.ExecuteCommandAsync(new GetRoadmapWorkspaceQuery(userRoadmapId), cancellationToken);
        return Results.Ok(GetRoadmapWorkspaceResponse.Create(result));
    })
        .Produces<GetRoadmapWorkspaceResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}
