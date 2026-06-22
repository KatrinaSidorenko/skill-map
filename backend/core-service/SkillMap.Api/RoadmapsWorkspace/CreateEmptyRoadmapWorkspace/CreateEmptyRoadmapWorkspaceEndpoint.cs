using LearningPlatform.Shared.Api;

using Microsoft.AspNetCore.Mvc;

using SkillMap.Api.Roadmaps;
using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalizedRoadmaps;
using SkillMap.Business.RoadmapsWorkspace.Features.CreateEmptyRoadmapWorkspace;
using SkillMap.Shared.Files;

namespace SkillMap.Api.RoadmapsWorkspace.CreateEmptyRoadmapWorkspace;

internal static class CreateEmptyRoadmapWorkspaceEndpoint
{
    internal static void MapCreateEmptyRoadmapWorkspace(this IEndpointRouteBuilder app) => app.MapPost(RoadmapsWorkspaceApiPaths.CreateEmptyRoadmapWorkspace, async (
        [FromForm] string title,
        [FromForm] string? description,
        [FromForm] IFormFile? imageFile,
        IUserManager userManager,
        IRoadmapWorkspaceModule roadmapWorkspaceModule,
        CancellationToken cancellationToken) =>
    {
        var isValidImageFile = FilesValidatorExtensions.CreateImageFilesValidator().IsValidWithEmptyAllowed(imageFile, out _);
        if (!isValidImageFile)
        {
            return Results.BadRequest("Invalid image file. Allowed formats are jpg, jpeg, png and the maximum size is 5MB.");
        }

        var hardFile = await imageFile.ToHardFileAsync(cancellationToken);
        var command = new CreateEmptyRoadmapWorkspaceCommand(userManager.GetCurrentUserId(), title, description, hardFile);
        var workspaceId = await roadmapWorkspaceModule.ExecuteCommandAsync(command, cancellationToken);

        return Results.Created($"{RoadmapsWorkspaceApiPaths.GetRoadmapWorkspace.Replace("{userRoadmapId}", workspaceId.ToString())}", workspaceId);
    })
        .RequireAuthorization()
        .DisableAntiforgery()
        .ValidateRequest<CreateEmptyRoadmapWorkspaceRequestValidator>()
        .Produces<long>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError);
}
