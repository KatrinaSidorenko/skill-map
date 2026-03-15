using SkillMap.Api.Roadmaps;
using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalizedRoadmaps;
using SkillMap.Business.RoadmapsWorkspace.Features.CreateRoadmapWorkspace;

namespace SkillMap.Api.RoadmapsWorkspace.CreateRoadmapWorkspace;

internal static class CreateRoadmapWorkspaceEndpoint
{
    internal static void MapCreateRoadmapWorkspace(this IEndpointRouteBuilder app) => app.MapPost(RoadmapsWorkspaceApiPaths.CreateRoadmapWorkspace, async (
        [AsParameters] CreateRoadmapWorkspaceRequest request,
        IUserManager userManager,
        IRoadmapWorkspaceModule roadmapWorkspaceModule,
        CancellationToken cancellationToken) =>
    {
        var command = new CreateRoadmapWorkspaceCommand(userManager.GetCurrentUserId(), request.RoadmapId);
        var workspaceId = await roadmapWorkspaceModule.ExecuteCommandAsync(command, cancellationToken);

        return Results.Created($"{RoadmapsWorkspaceApiPaths.GetRoadmapWorkspace.Replace("{userRoadmapId}", workspaceId.ToString())}", workspaceId);
    })
        .Produces<long>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status500InternalServerError);
}
