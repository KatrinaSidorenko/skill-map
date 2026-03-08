using SkillMap.Api.Roadmaps;
using SkillMap.Business.PersonalizedRoadmaps;
using SkillMap.Business.PersonalizedRoadmaps.Features.GetPersonalizedRoadmap;

namespace SkillMap.Api.PersonalizedRoadmaps.GetPersonalizedRoadmap;

internal static class GetRoadmapWorkspaceEndpoint
{
    internal static void MapGetPersonalizedRoadmap(this IEndpointRouteBuilder app) => app.MapGet(PersonalizedRoadmapsApiPaths.GetPersonalizedRoadmap, async (
            long userRoadmapId,
            IPersonalizedRoadmapModule personalizedRoadmapsModule, CancellationToken cancellationToken) =>
    {
        var result = await personalizedRoadmapsModule.ExecuteCommandAsync(new GetRoadmapWorkspaceQuery(userRoadmapId), cancellationToken);
        return Results.Ok(GetRoadmapWorkspaceResponse.Create(result));
    })
        .Produces<GetRoadmapWorkspaceResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}
