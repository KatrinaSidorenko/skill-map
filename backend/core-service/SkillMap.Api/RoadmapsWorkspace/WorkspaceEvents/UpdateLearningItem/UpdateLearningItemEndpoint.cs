using LearningPlatform.Shared.Api;

using SkillMap.Api.Roadmaps;
using SkillMap.Business.PersonalizedRoadmaps;

namespace SkillMap.Api.RoadmapsWorkspace.WorkspaceEvents.UpdateLearningItem;

internal static class UpdateLearningItemEndpoint
{
    internal static void MapUpdateLearningItem(this IEndpointRouteBuilder app) => app.MapPost(RoadmapsWorkspaceApiPaths.UpdateLearningItem, async (
            long userRoadmapId,
            UpdateLearningItemRequest request,
            IRoadmapWorkspaceModule roadmapWorkspaceModule, CancellationToken cancellationToken) =>
    {
        await roadmapWorkspaceModule.ExecuteCommandAsync(request.ToCommand(userRoadmapId), cancellationToken);
        return Results.NoContent();
    })
        .ValidateRequest<UpdateLearningItemRequestValidator>()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status500InternalServerError);
}
