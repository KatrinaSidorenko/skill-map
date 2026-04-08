using LearningPlatform.Shared.Api;

using SkillMap.Api.Roadmaps;
using SkillMap.Business.PersonalizedRoadmaps;

namespace SkillMap.Api.RoadmapsWorkspace.WorkspaceEvents.AddLearningItem;

internal static class AddLearningItemEndpoint
{
    internal static void MapAddLearningItem(this IEndpointRouteBuilder app) => app.MapPost(RoadmapsWorkspaceApiPaths.AddLearningItem, async (
            long userRoadmapId,
            AddLearningItemRequest request,
            IRoadmapWorkspaceModule personalizedRoadmapsModule, CancellationToken cancellationToken) =>
    {
        var command = request.ToCommand(userRoadmapId);
        await personalizedRoadmapsModule.ExecuteCommandAsync(command, cancellationToken);
        return Results.NoContent();
    })
        .ValidateRequest<AddLearningItemRequestValidator>()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status500InternalServerError);
}