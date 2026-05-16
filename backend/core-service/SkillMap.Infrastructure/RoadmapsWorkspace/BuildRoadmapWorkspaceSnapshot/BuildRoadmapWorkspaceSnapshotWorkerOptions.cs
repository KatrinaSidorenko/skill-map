namespace SkillMap.Infrastructure.RoadmapsWorkspace.BuildRoadmapWorkspaceSnapshot;
internal class BuildRoadmapWorkspaceSnapshotWorkerOptions
{
    internal const string SectionName = "BuildRoadmapWorkspaceSnapshotWorker";
    public TimeSpan ScheduleInterval { get; set; }
}
