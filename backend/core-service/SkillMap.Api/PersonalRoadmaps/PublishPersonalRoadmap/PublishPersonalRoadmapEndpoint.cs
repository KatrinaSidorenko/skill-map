using SkillMap.Business.PersonalRoadmaps;
using SkillMap.Business.PersonalRoadmaps.Features.PublishPersonalRoadmap;

namespace SkillMap.Api.PersonalRoadmaps.PublishPersonalRoadmap;

internal static class PublishPersonalRoadmapEndpoint
{
    internal static void MapPublishPersonalRoadmap(this IEndpointRouteBuilder app) => app.MapPost(PersonalRoadmapsApiPaths.PublishPersonalRoadmap, async (
           long personalRoadmapId,
              IRoadmapWorkspaceModule personalRoadmapModule,
              CancellationToken cancellationToken) =>
      {
          await personalRoadmapModule.ExecuteCommandAsync(new PublishPersonalRoadmapCommand(personalRoadmapId), cancellationToken);
          return Results.NoContent();
      })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
}
