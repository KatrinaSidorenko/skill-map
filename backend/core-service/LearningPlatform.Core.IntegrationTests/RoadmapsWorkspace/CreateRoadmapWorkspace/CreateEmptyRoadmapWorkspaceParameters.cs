using SkillMap.Api.RoadmapsWorkspace.CreateEmptyRoadmapWorkspace;
using SkillMap.Api.RoadmapsWorkspace.CreateRoadmapWorkspace;

namespace LearningPlatform.Core.IntegrationTests.RoadmapsWorkspace.CreateRoadmapWorkspace;
internal sealed record CreateEmptyRoadmapWorkspaceParameters(string Title, string Description)
{
    internal static CreateEmptyRoadmapWorkspaceParameters GetValid() => new(
        Title: "Test workspace",
        Description: "Test description"
    );
}

internal static class RoadmapWorkspaceRequestFaker
{
    internal static CreateEmptyRoadmapWorkspaceRequest GetRequest(this CreateEmptyRoadmapWorkspaceParameters parameters) => new CreateEmptyRoadmapWorkspaceRequest
    {
        Title = parameters.Title,
        Description = parameters.Description
    };
}
