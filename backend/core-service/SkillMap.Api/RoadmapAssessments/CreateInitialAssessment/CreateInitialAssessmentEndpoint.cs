using SkillMap.Business.RoadmapAssessments;
using SkillMap.Business.RoadmapAssessments.Features.CreateInitialAssessment;

namespace SkillMap.Api.RoadmapAssessments.CreateInitialAssessment;

internal static class CreateInitialAssessmentEndpoint
{
    internal static void MapCreateInitialAssessment(this IEndpointRouteBuilder app) =>
        app.MapPost(RoadmapAssessmentsApiPaths.CreateInitialAssessment, async (
        long workspaceId,
        IRoadmapAssessmentModule assessmentModule,
        CancellationToken cancellationToken) =>
        {
            var command = new CreateInitialAssessmentCommand(workspaceId);
            var assessmentId = await assessmentModule.ExecuteCommandAsync(command, cancellationToken);

            return Results.Created(
            $"{RoadmapAssessmentsApiPaths.CreateInitialAssessment.Replace("{workspaceId}", workspaceId.ToString())}/{assessmentId}",
            assessmentId);
        })
        .Produces<long>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}
