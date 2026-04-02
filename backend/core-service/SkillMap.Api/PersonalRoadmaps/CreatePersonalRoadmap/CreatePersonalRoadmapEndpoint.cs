using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalRoadmaps;
using LearningPlatform.Shared.Api;

namespace SkillMap.Api.PersonalRoadmaps.CreatePersonalRoadmap;

internal static class CreatePersonalRoadmapEndpoint
{
    internal static void MapCreatePersonalRoadmap(this IEndpointRouteBuilder app) => app.MapPost(PersonalRoadmapsApiPaths.CreatePersonalRoadmap, async (
            CreatePersonalRoadmapRequest request,
            IUserManager userManager,
            IRoadmapWorkspaceModule personalizedRoadmapsModule, CancellationToken cancellationToken) =>
    {
        var command = request.ToCommand(userManager.GetCurrentUserId());
        var roadmapId = await personalizedRoadmapsModule.ExecuteCommandAsync(command, cancellationToken);

        return Results.Created($"{PersonalRoadmapsApiPaths.CreatePersonalRoadmap}/{roadmapId}", roadmapId);
    })
        .ValidateRequest<CreatePersonalRoadmapRequestValidator>()
        .Produces<long>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status500InternalServerError);
}
