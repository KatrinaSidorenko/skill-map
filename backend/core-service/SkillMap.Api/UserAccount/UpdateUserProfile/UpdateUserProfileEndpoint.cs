using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.UserAccount.Features.UpdateUserProfile;

namespace SkillMap.Api.UserAccount.UpdateUserProfile;

internal static class UpdateUserProfileEndpoint
{
    internal static void MapUpdateUserProfile(this IEndpointRouteBuilder app) => app.MapPatch(UserAccountApiPaths.UpdateUserProfile, async (
            UpdateUserProfileRequest request,
            IUserManager userManager,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var userId = userManager.GetCurrentUserId();
            await mediator.Send(new UpdateUserProfileCommand(userId, request.UserName, request.Email, request.ImageUrl), cancellationToken);
            return Results.NoContent();
        })
        .RequireAuthorization()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);
}
