using SkillMap.Business.RoadmapAssessments;
using SkillMap.Business.RoadmapAssessments.Features.EvaluateRoadmapAssessment;

namespace SkillMap.Api.RoadmapAssessments.EvaluateRoadmapAssessment;

internal static class EvaluateRoadmapAssessmentEndpoint
{
    internal static void MapEvaluateRoadmapAssessment(this IEndpointRouteBuilder app) =>
        app.MapPost(RoadmapAssessmentsApiPaths.EvaluateAssessment, async (
            long attemptId,
            EvaluateRoadmapAssessmentRequest request,
            IRoadmapAssessmentModule assessmentModule,
            CancellationToken cancellationToken) =>
        {
            var command = new EvaluateRoadmapAssessmentCommand(
                attemptId,
                request.ProvidedAnswers
                .Select(a => new ProvidedAnswer(a.QuestionId, a.Type, a.SelectedAnswerId))
                .ToList());

            await assessmentModule.ExecuteCommandAsync(command, cancellationToken);

            return Results.Ok();
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}