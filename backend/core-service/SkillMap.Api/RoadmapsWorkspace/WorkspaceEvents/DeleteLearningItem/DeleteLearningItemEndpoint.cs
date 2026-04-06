using LearningPlatform.Shared.Api;

using SkillMap.Api.Roadmaps;
using SkillMap.Business.PersonalizedRoadmaps;

namespace SkillMap.Api.RoadmapsWorkspace.WorkspaceEvents.DeleteLearningItem;

internal static class DeleteLearningItemEndpoint
{
    internal static void MapDeleteLearningItem(this IEndpointRouteBuilder app) => app.MapPost(RoadmapsWorkspaceApiPaths.DeleteLearningItem, async (
        long userRoadmapId,
        DeleteLearningItemRequest request,
        IRoadmapWorkspaceModule roadmapWorkspaceModule,
        CancellationToken cancellationToken) =>
    {
        await roadmapWorkspaceModule.ExecuteCommandAsync(request.ToCommand(userRoadmapId), cancellationToken);
        return Results.NoContent();
    })
        .ValidateRequest<DeleteLearningItemRequestValidator>()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}