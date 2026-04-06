using LearningPlatform.Shared.Api;

using SkillMap.Api.Roadmaps;
using SkillMap.Business.PersonalizedRoadmaps;

namespace SkillMap.Api.RoadmapsWorkspace.WorkspaceEvents.CreateLearningItemConnection;

internal static class CreateLearningItemConnectionEndpoint
{
    internal static void MapCreateLearningItemConnection(this IEndpointRouteBuilder app) => app.MapPost(RoadmapsWorkspaceApiPaths.CreateLearningItemConnection, async (
        long userRoadmapId,
        CreateLearningItemConnectionRequest request,
        IRoadmapWorkspaceModule roadmapWorkspaceModule,
        CancellationToken cancellationToken) =>
    {
        await roadmapWorkspaceModule.ExecuteCommandAsync(request.ToCommand(userRoadmapId), cancellationToken);
        return Results.NoContent();
    })
        .ValidateRequest<CreateLearningItemConnectionRequestValidator>()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status500InternalServerError);
}