using SkillMap.Api.Roadmaps;
using SkillMap.Business.PersonalizedRoadmaps;
using SkillMap.Business.RoadmapsWorkspace.Features.UpdateRoadmapWorkspace;

namespace SkillMap.Api.RoadmapsWorkspace.UpdateRoadmapWorkspace;

internal static class UpdateRoadmapWorkspaceEndpoint
{
    internal static void MapUpdateRoadmapWorkspace(this IEndpointRouteBuilder app) => app.MapPatch(RoadmapsWorkspaceApiPaths.UpdateRoadmapWorkspace, async (
        long userRoadmapId,
        UpdateRoadmapWorkspaceRequest request,
        IRoadmapWorkspaceModule roadmapWorkspaceModule,
        CancellationToken cancellationToken) =>
    {
        var command = new UpdateRoadmapWorkspaceCommand(userRoadmapId, request.Title, request.Description, request.ImageUrl);
        await roadmapWorkspaceModule.ExecuteCommandAsync(command, cancellationToken);
        return Results.NoContent();
    })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}
