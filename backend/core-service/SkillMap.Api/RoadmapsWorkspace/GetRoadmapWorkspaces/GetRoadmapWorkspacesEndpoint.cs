using LearningPlatform.Shared.Api.Searching;

using SkillMap.Api.Roadmaps;
using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalizedRoadmaps;
using SkillMap.Business.RoadmapsWorkspace.Features.GetRoadmapWorkspaces;

namespace SkillMap.Api.RoadmapsWorkspace.GetRoadmapWorkspaces;

internal static class GetRoadmapWorkspacesEndpoint
{
    internal static void MapGetRoadmapWorkspaces(this IEndpointRouteBuilder app) => app.MapGet(RoadmapsWorkspaceApiPaths.GetRoadmapWorkspaces, async (
            [AsParameters] FilteringRequest filteringRequest,
            IRoadmapWorkspaceModule roadmapWorkspaceModule,
            IUserManager userManager,
            CancellationToken cancellationToken) =>
    {
        var query = new GetRoadmapWorkspacesQuery(userManager.GetCurrentUserId(), filteringRequest.ToParams());
        var result = await roadmapWorkspaceModule.ExecuteCommandAsync(query, cancellationToken);
        return Results.Ok(GetRoadmapWorkspacesResponse.CreatePaginationResult(result));
    })
        .Produces<GetRoadmapWorkspacesResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}
