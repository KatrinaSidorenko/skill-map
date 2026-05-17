using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.UserAccount.Features.GetUserDashboard;

namespace SkillMap.Api.UserAccount.GetUserDashboard;

internal static class GetUserDashboardEndpoint
{
    internal static void MapGetUserDashboard(this IEndpointRouteBuilder app) =>
        app.MapGet(UserAccountApiPaths.GetUserDashboard, async (
            IUserManager userManager,
            IMediator mediator,
            CancellationToken cancellationToken) =>
    {
        var userId = userManager.GetCurrentUserId();
        var dto = await mediator.Send(new GetUserDashboardQuery(userId), cancellationToken);
        return Results.Ok(UserDashboardResponse.Create(dto));
    })
        .RequireAuthorization()
        .Produces<UserDashboardResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .WithTags("UserAccount");
}
