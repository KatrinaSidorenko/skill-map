using SkillMap.Api.RoadmapAssessments.GetRoadmapAssessment;
using SkillMap.Business.RoadmapAssessments;
using SkillMap.Business.RoadmapAssessments.Features.GetAssessment;

namespace SkillMap.Api.RoadmapAssessments.GetAssessment;

internal static class GetAssessmentEndpoint
{
    internal static void MapGetAttemptAssessment(this IEndpointRouteBuilder app) =>
            app.MapGet(RoadmapAssessmentsApiPaths.GetAttemptAssessmentContent, async (
            long attemptId,
            IRoadmapAssessmentModule assessmentModule,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAttemptAssessmentContentQuery(attemptId);
            var dto = await assessmentModule.ExecuteCommandAsync(query, cancellationToken);
            return Results.Ok(RoadmapAssessmentResponse.Create(dto));
        })
        .Produces<RoadmapAssessmentResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}