using SkillMap.Business.RoadmapAssessments;
using SkillMap.Business.RoadmapAssessments.Features.GetAssessmentAttemptResult;

namespace SkillMap.Api.RoadmapAssessments.GetAssessmentAttemptResult;

internal static class GetAssessmentAttemptResultEndpoint
{
    internal static void MapGetAssessmentAttemptResult(this IEndpointRouteBuilder app) =>
            app.MapGet(RoadmapAssessmentsApiPaths.GetAssessmentAttemptResult, async (
            long attemptId,
            IRoadmapAssessmentModule assessmentModule,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAssessmentAttemptResultQuery(attemptId);
            var dto = await assessmentModule.ExecuteCommandAsync(query, cancellationToken);
            return Results.Ok(AssessmentAttemptResultResponse.Create(dto));
        })
            .Produces<AssessmentAttemptResultResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();
}