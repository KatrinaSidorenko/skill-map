using SkillMap.Business.RoadmapAssessments;
using SkillMap.Business.RoadmapAssessments.Features.CreateIntermediateAssessment;

namespace SkillMap.Api.RoadmapAssessments.CreateIntermediateAssessment;

internal static class CreateIntermediateAssessmentEndpoint
{
    internal static void MapCreateIntermediateAssessment(this IEndpointRouteBuilder app) =>
        app.MapPost(RoadmapAssessmentsApiPaths.CreateIntermediateAssessment, async (
        long workspaceId,
        IRoadmapAssessmentModule assessmentModule,
        CancellationToken cancellationToken) =>
        {
            var command = new CreateIntermediateAssessmentCommand(workspaceId);
            var assessmentId = await assessmentModule.ExecuteCommandAsync(command, cancellationToken);

            return Results.Created(
                    $"{RoadmapAssessmentsApiPaths.CreateIntermediateAssessment.Replace("{workspaceId}", workspaceId.ToString())}/{assessmentId}",
                   assessmentId);
        })
        .Produces<long>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}