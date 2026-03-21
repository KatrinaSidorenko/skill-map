using SkillMap.Api.Roadmaps;
using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalizedRoadmaps;
using SkillMap.Business.RoadmapsWorkspace.Features.DeleteRoadmapFork;

namespace SkillMap.Api.RoadmapsWorkspace.DeleteRoadmapWorkspace;

internal static class DeleteRoadmapWorkspaceEndpoint
{
    internal static void MapDeleteRoadmapWorkspace(this IEndpointRouteBuilder app) => app.MapDelete(RoadmapsWorkspaceApiPaths.DeleteRoadmapWorkspace, async (
        string roadmapId,
        IUserManager userManager,
        IRoadmapWorkspaceModule roadmapWorkspaceModule,
        CancellationToken cancellationToken) =>
    {
        await roadmapWorkspaceModule.ExecuteCommandAsync(new DeleteWorkspaceCommand(roadmapId, userManager.GetCurrentUserId()), cancellationToken);
        return Results.NoContent();
    })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}
