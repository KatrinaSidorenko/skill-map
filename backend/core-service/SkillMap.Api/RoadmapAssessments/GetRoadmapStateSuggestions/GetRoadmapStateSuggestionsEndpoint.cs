using SkillMap.Business.RoadmapAssessments;
using SkillMap.Business.RoadmapAssessments.Features.GetRoadmapStateSuggestions;

namespace SkillMap.Api.RoadmapAssessments.GetRoadmapStateSuggestions;

internal static class GetRoadmapStateSuggestionsEndpoint
{
    internal static void MapGetRoadmapStateSuggestions(this IEndpointRouteBuilder app) =>
            app.MapGet(RoadmapAssessmentsApiPaths.GetRoadmapStateSuggestions, async (
            long attemptId,
            IRoadmapAssessmentModule assessmentModule,
            CancellationToken cancellationToken) =>
        {
            var query = new GetRoadmapStateSuggestionsQuery(attemptId);
            var dto = await assessmentModule.ExecuteCommandAsync(query, cancellationToken);
            return Results.Ok(RoadmapStateSuggestionsResponse.Create(dto));
        })
            .Produces<RoadmapStateSuggestionsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();
}
