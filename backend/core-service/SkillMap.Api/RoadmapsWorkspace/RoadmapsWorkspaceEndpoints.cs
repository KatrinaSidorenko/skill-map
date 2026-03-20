using SkillMap.Api.PersonalizedRoadmaps.GetPersonalizedRoadmap;
using SkillMap.Api.RoadmapsWorkspace.CreateRoadmapWorkspace;
using SkillMap.Api.RoadmapsWorkspace.GetRoadmapWorkspaces;
using SkillMap.Api.RoadmapsWorkspace.GetRoadmapWorkspaceSummary;
using SkillMap.Api.RoadmapsWorkspace.WorkspaceEvents.AddLearningItem;
using SkillMap.Api.RoadmapsWorkspace.WorkspaceEvents.CreateLearningItemConnection;
using SkillMap.Api.RoadmapsWorkspace.WorkspaceEvents.DeleteLearningItem;
using SkillMap.Api.RoadmapsWorkspace.WorkspaceEvents.UpdateLearningItem;
using SkillMap.Api.RoadmapsWorkspace.WorkspaceEvents.DeleteLearningItemConnection;

namespace SkillMap.Api.Roadmap;

internal static class RoadmapsWorkspaceEndpoints
{
    internal static void MapRoadmapsWorkspace(this WebApplication app)
    {
        app.MapGetRoadmapWorkspaces();
        app.MapGetRoadmapWorkspace();
        app.MapGetRoadmapWorkspaceSummary();
        app.MapCreateRoadmapWorkspace();

        app.MapAddLearningItem();
        app.MapUpdateLearningItem();
        app.MapDeleteLearningItem();
        app.MapCreateLearningItemConnection();
        app.MapDeleteLearningItemConnection();
    }
}
