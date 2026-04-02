using LearningPlatform.Shared.Api.Searching;

using Microsoft.AspNetCore.Mvc;

using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalRoadmaps;
using SkillMap.Business.PersonalRoadmaps.Features.GetPersonalRoadmaps;

namespace SkillMap.Api.PersonalRoadmaps.GetPersonalRoadmaps;

internal static class GetPersonalRoadmapsEndpoint
{
    internal static void MapGetPersonalRoadmaps(this IEndpointRouteBuilder app) => app.MapGet(PersonalRoadmapsApiPaths.GetPersonalRoadmaps, async (
        [AsParameters] FilteringRequest filteringRequest,
        IRoadmapWorkspaceModule personalRoadmapModule,
        IUserManager userManager, CancellationToken cancellationToken) =>
    {
        var query = new GetPersonalRoadmapsQuery(userManager.GetCurrentUserId(), filteringRequest.ToParams());
        var result = await personalRoadmapModule.ExecuteCommandAsync(query, cancellationToken);
        return Results.Ok(GetPersonalRoadmapsResponse.CreatePaginationResult(result));
    })
        .Produces<GetPersonalRoadmapsResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}
