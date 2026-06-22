using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;

namespace SkillMap.Business.RoadmapsWorkspace;
public interface IRoadmapWorkspaceEditor
{
    Task<RoadmapSnapshot> GetActualRoadmapWorkspaceSnapshot(long workspaceId, CancellationToken cancellationToken);
}