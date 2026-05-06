using Microsoft.AspNetCore.Mvc;

using SkillMap.Api.Roadmaps;
using SkillMap.Business.PersonalizedRoadmaps;
using SkillMap.Business.RoadmapsWorkspace.Features.DeleteRoadmapFork;

namespace SkillMap.Api.RoadmapsWorkspace.DeleteRoadmapWorkspace;

internal static class DeleteRoadmapWorkspaceEndpoint
{
    internal static void MapDeleteRoadmapWorkspace(this IEndpointRouteBuilder app) => app.MapDelete(RoadmapsWorkspaceApiPaths.DeleteRoadmapWorkspace, async (
        long userRoadmapId,
        [FromQuery] bool? isSoftDelete,
        IRoadmapWorkspaceModule roadmapWorkspaceModule,
        CancellationToken cancellationToken) =>
    {
        isSoftDelete ??= true;
        await roadmapWorkspaceModule.ExecuteCommandAsync(new DeleteWorkspaceCommand(userRoadmapId, isSoftDelete.Value), cancellationToken);
        return Results.NoContent();
    })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}