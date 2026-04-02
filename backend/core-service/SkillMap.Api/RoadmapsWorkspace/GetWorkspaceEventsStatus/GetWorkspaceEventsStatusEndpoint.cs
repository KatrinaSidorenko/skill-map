using SkillMap.Api.Roadmaps;
using SkillMap.Business.PersonalizedRoadmaps;
using SkillMap.Business.RoadmapsWorkspace.Features.GetWorkspaceEventsStatus;

namespace SkillMap.Api.RoadmapsWorkspace.GetWorkspaceEventsStatus;

internal static class GetWorkspaceEventsStatusEndpoint
{
    internal static void MapGetWorkspaceEventsStatus(this IEndpointRouteBuilder app) =>
         app.MapPost(RoadmapsWorkspaceApiPaths.GetWorkspaceEventsStatus, async (
            long userRoadmapId,
            GetWorkspaceEventsStatusRequest request,
            IRoadmapWorkspaceModule roadmapWorkspaceModule,
            CancellationToken cancellationToken) =>
        {
            var query = new GetWorkspaceEventsStatusQuery(userRoadmapId, request.IdempotencyKeys);
            var result = await roadmapWorkspaceModule.ExecuteCommandAsync(query, cancellationToken);
            return Results.Ok(GetWorkspaceEventsStatusResponse.Create(result));
        })
        .Produces<GetWorkspaceEventsStatusResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}
