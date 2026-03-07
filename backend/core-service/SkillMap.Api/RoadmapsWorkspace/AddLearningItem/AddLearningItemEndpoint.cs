using SkillMap.Api.Roadmaps;
using SkillMap.Business.PersonalizedRoadmaps;
using LearningPlatform.Shared.Api;

namespace SkillMap.Api.PersonalizedRoadmaps.AddLearningItem;

internal static class AddLearningItemEndpoint
{
    internal static void MapAddLearningItem(this IEndpointRouteBuilder app) => app.MapPost(PersonalizedRoadmapsApiPaths.AddLearningItem, async (
            long userRoadmapId,
            AddLearningItemRequest request,
            IPersonalizedRoadmapModule personalizedRoadmapsModule, CancellationToken cancellationToken) =>
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
