using Microsoft.AspNetCore.Mvc;

using SkillMap.Api.Roadmaps;
using SkillMap.Business.PersonalizedRoadmaps;
using SkillMap.Business.RoadmapsWorkspace.Features.UpdateRoadmapWorkspace;
using SkillMap.Shared.Files;

namespace SkillMap.Api.RoadmapsWorkspace.UpdateRoadmapWorkspace;

internal static class UpdateRoadmapWorkspaceEndpoint
{
    internal static void MapUpdateRoadmapWorkspace(this IEndpointRouteBuilder app) => app.MapPatch(RoadmapsWorkspaceApiPaths.UpdateRoadmapWorkspace, async (
        [FromRoute]long userRoadmapId,
        [FromForm] string? title,
        [FromForm] string? description,
        [FromForm] IFormFile? imageFile,
        IRoadmapWorkspaceModule roadmapWorkspaceModule,
        CancellationToken cancellationToken) =>
    {
        var isFileValid = FilesValidatorExtensions.CreateImageFilesValidator().IsValidWithEmptyAllowed(imageFile, out _);
        if (!isFileValid)
        {
            return Results.BadRequest("Invalid image file.");
        }

        var hardFile = await imageFile.ToHardFileAsync(cancellationToken);
        var command = new UpdateRoadmapWorkspaceCommand(userRoadmapId, title, description, hardFile);
        await roadmapWorkspaceModule.ExecuteCommandAsync(command, cancellationToken);
        return Results.NoContent();
    })
        .RequireAuthorization()
        .DisableAntiforgery()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}
