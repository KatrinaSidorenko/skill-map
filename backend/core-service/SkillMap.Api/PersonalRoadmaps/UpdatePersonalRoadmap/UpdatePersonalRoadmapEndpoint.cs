using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalRoadmaps;

namespace SkillMap.Api.PersonalRoadmaps.UpdatePersonalRoadmap;

internal static class UpdatePersonalRoadmapEndpoint
{
    internal static void MapUpdatePersonalRoadmap(this IEndpointRouteBuilder app) => app.MapPut(PersonalRoadmapsApiPaths.UpdatePersonalRoadmap, async (
        long personalRoadmapId,
        UpdatePersonalRoadmapRequest request,
        IUserManager userManager,
        IRoadmapWorkspaceModule personalRoadmapModule,
        CancellationToken cancellationToken) =>
    {
        var command = request.ToCommand(personalRoadmapId, userManager.GetCurrentUserId());
        await personalRoadmapModule.ExecuteCommandAsync(command, cancellationToken);
        return Results.NoContent();
    })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}