using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;

namespace LearningPlatform.Core.UnitTests.CreateWorkspaceSnapshot;
public static class BuildWorkspaceSnapshotExtension
{
    public static RoadmapWorkspaceSnapshot WithNavigationWorkspace(this RoadmapWorkspaceSnapshot snapshot, long workspaceId)
    {
        snapshot.RoadmapWorkspace = new SkillMap.Core.RoadmapsWorkspace.RoadmapWorkspace { Id = workspaceId };
        return snapshot;
    }
}
