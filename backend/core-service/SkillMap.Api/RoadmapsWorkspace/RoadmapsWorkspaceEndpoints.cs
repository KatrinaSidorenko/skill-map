using LearningPlatform.Workspace.WebSockets;

using SkillMap.Api.PersonalizedRoadmaps.GetPersonalizedRoadmap;
using SkillMap.Api.RoadmapsWorkspace.CreateRoadmapWorkspace;
using SkillMap.Api.RoadmapsWorkspace.DeleteRoadmapWorkspace;
using SkillMap.Api.RoadmapsWorkspace.GetRoadmapWorkspaces;
using SkillMap.Api.RoadmapsWorkspace.GetRoadmapWorkspaceSummary;

namespace SkillMap.Api.Roadmap;

internal static class RoadmapsWorkspaceEndpoints
{
    internal static void MapRoadmapsWorkspace(this WebApplication app)
    {
        app.MapGetRoadmapWorkspaces();
        app.MapGetRoadmapWorkspace();
        app.MapGetRoadmapWorkspaceSummary();
        app.MapCreateRoadmapWorkspace();
        app.MapDeleteRoadmapWorkspace();

        app.MapHub<WorkspaceHub>("api/hubs/workspace");
    }
}