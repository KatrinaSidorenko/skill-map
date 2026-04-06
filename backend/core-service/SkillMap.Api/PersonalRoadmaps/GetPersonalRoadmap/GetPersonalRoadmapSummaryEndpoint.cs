using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalRoadmaps;
using SkillMap.Business.PersonalRoadmaps.Features.GetPersonalRoadmap;

namespace SkillMap.Api.PersonalRoadmaps.GetPersonalRoadmap;

internal static class GetPersonalRoadmapSummaryEndpoint
{
    internal static void MapGetPersonalRoadmapSummary(this IEndpointRouteBuilder app) => app.MapGet(PersonalRoadmapsApiPaths.GetPersonalRoadmap, async (
        string personalRoadmapId,
        IUserManager userManager,
        IRoadmapWorkspaceModule personalRoadmapModule,
        CancellationToken cancellationToken) =>
    {
        var query = new GetPersonalRoadmapSummaryQuery(personalRoadmapId, userManager.GetCurrentUserId());
        var result = await personalRoadmapModule.ExecuteCommandAsync(query, cancellationToken);
        return Results.Ok(GetPersonalRoadmapSummaryResponse.Create(result));
    })
        .Produces<GetPersonalRoadmapSummaryResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}