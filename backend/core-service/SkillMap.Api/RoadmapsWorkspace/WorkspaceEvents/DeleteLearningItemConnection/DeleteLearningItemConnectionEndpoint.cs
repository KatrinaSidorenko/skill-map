using LearningPlatform.Shared.Api;

using SkillMap.Api.Roadmaps;
using SkillMap.Business.PersonalizedRoadmaps;

namespace SkillMap.Api.RoadmapsWorkspace.WorkspaceEvents.DeleteLearningItemConnection;

internal static class DeleteLearningItemConnectionEndpoint
{
    internal static void MapDeleteLearningItemConnection(this IEndpointRouteBuilder app) => app.MapPost(RoadmapsWorkspaceApiPaths.DeleteLearningItemConnection, async (
        long userRoadmapId,
        DeleteLearningItemConnectionRequest request,
        IRoadmapWorkspaceModule roadmapWorkspaceModule,
        CancellationToken cancellationToken) =>
    {
        await roadmapWorkspaceModule.ExecuteCommandAsync(request.ToCommand(userRoadmapId), cancellationToken);
        return Results.NoContent();
    })
        .ValidateRequest<DeleteLearningItemConnectionRequestValidator>()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}
