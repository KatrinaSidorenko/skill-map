using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapAssessments;
using SkillMap.Business.RoadmapAssessments.Features.CreateAssessmentAttempt;

namespace SkillMap.Api.RoadmapAssessments.CreateAssessmentAttempt;

internal static class CreateAssessmentAttemptEndpoint
{
    internal static void MapCreateAssessmentAttempt(this IEndpointRouteBuilder app) =>
        app.MapPost(RoadmapAssessmentsApiPaths.CreateAssessmentAttempt, async (
            long assessmentId,
            IRoadmapAssessmentModule assessmentModule,
            IUserManager userManager,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateAssessmentAttemptCommand(assessmentId, userManager.GetCurrentUserId());
            var attemptId = await assessmentModule.ExecuteCommandAsync(command, cancellationToken);

            return Results.Created(
                RoadmapAssessmentsApiPaths.CreateAssessmentAttempt.Replace("{assessmentId}", assessmentId.ToString()),
                CreateAssessmentAttemptResponse.Create(attemptId));
        })
            .Produces<CreateAssessmentAttemptResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();
}