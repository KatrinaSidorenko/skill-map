using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.UserAccount.Features.GetUserProfile;

namespace SkillMap.Api.UserAccount.GetUserProfile;

internal static class GetUserProfileEndpoint
{
    internal static void MapGetUserProfile(this IEndpointRouteBuilder app) => app.MapGet(UserAccountApiPaths.GetUserProfile, async (
       IUserManager userManager,
   IMediator mediator,
            CancellationToken cancellationToken) =>
   {
       var userId = userManager.GetCurrentUserId();
       var dto = await mediator.Send(new GetUserProfileQuery(userId), cancellationToken);
       var response = new UserProfileResponse
       {
           Id = dto.Id,
           UserName = dto.UserName,
           Email = dto.Email,
           ImageUrl = dto.ImageUrl,
           Role = dto.Role
       };
       return Results.Ok(response);
   })
    .RequireAuthorization()
    .Produces<UserProfileResponse>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status404NotFound);
}
