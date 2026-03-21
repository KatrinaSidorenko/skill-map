using LearningPlatform.Shared.Api.Searching;

using Microsoft.AspNetCore.Mvc;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapBlueprints;
using SkillMap.Business.RoadmapBlueprints.GetRoadmapBlueprintSummary;
using SkillMap.Shared.Models;

namespace SkillMap.Api.RoadmapBlueprints.GetRoadmapBlueprintsSummary;

internal static class GetRoadmapBlueprintsSummaryEndpoint
{
    internal static void MapGetRoadmapBlueprintsSummary(this IEndpointRouteBuilder app) => app.MapGet(RoadmapBlueprintsApiPaths.GetRoadmapBlueprintsSummary, async (
        [AsParameters] FilteringRequest request,
        IRoadmapBlueprintModule roadmapBlueprintModule,
        IUserManager userManager,
        CancellationToken cancellationToken) =>
    {
        var query = new GetRoadmapBlueprintSummaryQuery(request.ToParams(), userManager.GetCurrentUserId());
        var result = await roadmapBlueprintModule.ExecuteCommandAsync(query, cancellationToken);
        return Results.Ok(GetRoadmapBlueprintsSummaryResponse.CreatePaginationResult(result));
    })
        .Produces<GetRoadmapBlueprintsSummaryResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}
