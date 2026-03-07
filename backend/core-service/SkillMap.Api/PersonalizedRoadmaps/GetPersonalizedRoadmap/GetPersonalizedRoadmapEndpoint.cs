using SkillMap.Api.Roadmaps;
using SkillMap.Business.PersonalizedRoadmaps;
using SkillMap.Business.PersonalizedRoadmaps.GetPersonalizedRoadmap;

namespace SkillMap.Api.PersonalizedRoadmaps.GetPersonalizedRoadmap;

internal static class GetPersonalizedRoadmapEndpoint
{
    internal static void MapGetPersonalizedRoadmap(this IEndpointRouteBuilder app) => app.MapGet(PersonalizedRoadmapsApiPaths.GetPersonalizedRoadmap, async (
            long userRoadmapId,
            IPersonalizedRoadmapModule personalizedRoadmapsModule, CancellationToken cancellationToken) =>
    {
        var result = await personalizedRoadmapsModule.ExecuteCommandAsync(new GetPersonalizedRoadmapQuery(userRoadmapId), cancellationToken);
        return Results.Ok(GetPersonalizedRoadmapResponse.Create(result));
    })
        .Produces<GetPersonalizedRoadmapResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}
