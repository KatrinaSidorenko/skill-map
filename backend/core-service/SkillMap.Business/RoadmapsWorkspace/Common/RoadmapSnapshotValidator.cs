using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;

namespace SkillMap.Business.RoadmapsWorkspace.Common;
public interface RoadmapSnapshotValidator
{
    public async Task<bool> IsRoadmapStateValid(RoadmapSnapshot roadmapSnapshot, CancellationToken cancellationToken)
    {
        var hasCycles = HasCycles(roadmapSnapshot);
        return hasCycles ? false : true;
    }

    private bool HasCycles(RoadmapSnapshot roadmapSnapshot)
        => TopologicalSort.HasCycle(roadmapSnapshot);
}