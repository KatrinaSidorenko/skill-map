using SkillMap.Business.RoadmapAssessments;

namespace SkillMap.Api.RoadmapAssessments.ApplyRoadmapStateSuggestions;

internal static class ApplyRoadmapStateSuggestionsEndpoint
{
    internal static void MapApplyRoadmapStateSuggestions(this IEndpointRouteBuilder app) =>
            app.MapPost(RoadmapAssessmentsApiPaths.ApplyRoadmapStateSuggestions, async (
            long attemptId,
            ApplyRoadmapStateSuggestionsRequest request,
            IRoadmapAssessmentModule assessmentModule,
            CancellationToken cancellationToken) =>
        {
            await assessmentModule.ExecuteCommandAsync(
                   request.ToCommand(attemptId), cancellationToken);

            return Results.NoContent();
        })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();
}
