using SkillMap.Business.RoadmapAssessments;
using SkillMap.Business.RoadmapAssessments.Features.GetAssessmentHistory;

namespace SkillMap.Api.RoadmapAssessments.GetAssessmentHistory;

internal static class GetAssessmentHistoryEndpoint
{
    internal static void MapGetAssessmentHistory(this IEndpointRouteBuilder app) =>
        app.MapGet(RoadmapAssessmentsApiPaths.GetAssessmentHistory, async (
        long workspaceId,
        IRoadmapAssessmentModule assessmentModule,
        CancellationToken cancellationToken) =>
   {
       var query = new GetAssessmentHistoryQuery(workspaceId);
       var dto = await assessmentModule.ExecuteCommandAsync(query, cancellationToken);
       return Results.Ok(AssessmentHistoryResponse.Create(dto));
   })
        .Produces<AssessmentHistoryResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError)
        .RequireAuthorization();
}