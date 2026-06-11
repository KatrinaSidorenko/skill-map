using MediatR;

using Microsoft.AspNetCore.Mvc;

using SkillMap.Business.Abstractions;
using SkillMap.Business.UserAccount.Features.UpdateUserProfile;
using SkillMap.Shared.Files;

namespace SkillMap.Api.UserAccount.UpdateUserProfile;

internal static class UpdateUserProfileEndpoint
{
    internal static void MapUpdateUserProfile(this IEndpointRouteBuilder app) => app.MapPatch(UserAccountApiPaths.UpdateUserProfile, async (
            [FromForm] IFormFile? imageFile,
            [FromForm] string? username,
            [FromForm] string? email,
            IUserManager userManager,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var userId = userManager.GetCurrentUserId();
            var isFileValid = FilesValidatorExtensions.CreateImageFilesValidator().IsValidWithEmptyAllowed(imageFile, out _);
            if (!isFileValid)
            {
                return Results.BadRequest("Invalid image file.");
            }

            var hardFile = await imageFile.ToHardFileAsync(cancellationToken);
            await mediator.Send(new UpdateUserProfileCommand(userId, username, email, hardFile), cancellationToken);
            return Results.NoContent();
        })
        .RequireAuthorization()
        .DisableAntiforgery()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);
}
