using LearningPlatform.Workspace.WebSockets;

using SkillMap.Api.PersonalizedRoadmaps.GetPersonalizedRoadmap;
using SkillMap.Api.RoadmapsWorkspace.CreateEmptyRoadmapWorkspace;
using SkillMap.Api.RoadmapsWorkspace.CreateRoadmapWorkspace;
using SkillMap.Api.RoadmapsWorkspace.DeleteRoadmapWorkspace;
using SkillMap.Api.RoadmapsWorkspace.GetRoadmapWorkspaces;
using SkillMap.Api.RoadmapsWorkspace.GetRoadmapWorkspaceSummary;
using SkillMap.Api.RoadmapsWorkspace.UpdateRoadmapWorkspace;

namespace SkillMap.Api.Roadmap;

internal static class RoadmapsWorkspaceEndpoints
{
    internal static void MapRoadmapsWorkspace(this WebApplication app)
    {
        app.MapGetRoadmapWorkspaces();
        app.MapGetRoadmapWorkspace();
        app.MapGetRoadmapWorkspaceSummary();
        app.MapCreateRoadmapWorkspace();
        app.MapCreateEmptyRoadmapWorkspace();
        app.MapDeleteRoadmapWorkspace();
        app.MapUpdateRoadmapWorkspace();
    }
}