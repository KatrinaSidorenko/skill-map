using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;

namespace SkillMap.Business.RoadmapsWorkspace;
public interface IRoadmapWorkspaceEditor
{
    Task<RoadmapSnapshot> GetActualRoadmapSnapshot(long workspaceId, CancellationToken cancellationToken);
}