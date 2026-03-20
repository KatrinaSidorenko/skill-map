using SkillMap.Api.Roadmaps;
using SkillMap.Business.PersonalizedRoadmaps;
using SkillMap.Business.RoadmapsWorkspace.Features.GetRoadmapWorkspaceSummary;

namespace SkillMap.Api.RoadmapsWorkspace.GetRoadmapWorkspaceSummary;

internal static class GetRoadmapWorkspaceSummaryEndpoint
{
    internal static void MapGetRoadmapWorkspaceSummary(this IEndpointRouteBuilder app) => app.MapGet(RoadmapsWorkspaceApiPaths.GetRoadmapWorkspaceSummary, async (
         long userRoadmapId,
            IRoadmapWorkspaceModule roadmapWorkspaceModule,
            CancellationToken cancellationToken) =>
    {
        var result = await roadmapWorkspaceModule.ExecuteCommandAsync(new GetRoadmapWorkspaceSummaryQuery(userRoadmapId), cancellationToken);
        return Results.Ok(GetRoadmapWorkspaceSummaryResponse.Create(result));
    })
        .Produces<GetRoadmapWorkspaceSummaryResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}
