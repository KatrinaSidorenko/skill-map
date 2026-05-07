using LearningPlatform.Shared.Api;

using SkillMap.Api.Roadmaps;
using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalizedRoadmaps;
using SkillMap.Business.RoadmapsWorkspace.Features.CreateEmptyRoadmapWorkspace;

namespace SkillMap.Api.RoadmapsWorkspace.CreateEmptyRoadmapWorkspace;

internal static class CreateEmptyRoadmapWorkspaceEndpoint
{
    internal static void MapCreateEmptyRoadmapWorkspace(this IEndpointRouteBuilder app) => app.MapPost(RoadmapsWorkspaceApiPaths.CreateEmptyRoadmapWorkspace, async (
        CreateEmptyRoadmapWorkspaceRequest request,
        IUserManager userManager,
        IRoadmapWorkspaceModule roadmapWorkspaceModule,
        CancellationToken cancellationToken) =>
    {
        var command = new CreateEmptyRoadmapWorkspaceCommand(userManager.GetCurrentUserId(), request.Title, request.Description, request.ImageUrl);
        var workspaceId = await roadmapWorkspaceModule.ExecuteCommandAsync(command, cancellationToken);

        return Results.Created($"{RoadmapsWorkspaceApiPaths.GetRoadmapWorkspace.Replace("{userRoadmapId}", workspaceId.ToString())}", workspaceId);
    })
        .ValidateRequest<CreateEmptyRoadmapWorkspaceRequestValidator>()
        .Produces<long>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError);
}
