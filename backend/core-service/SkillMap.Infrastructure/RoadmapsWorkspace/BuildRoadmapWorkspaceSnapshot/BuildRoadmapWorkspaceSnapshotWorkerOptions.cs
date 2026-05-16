using System.Text.Json.Serialization;

using Newtonsoft.Json;

using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;

namespace SkillMap.Infrastructure.RoadmapsWorkspace.BuildRoadmapWorkspaceSnapshot;
internal class BuildRoadmapWorkspaceSnapshotWorkerOptions
{
    internal const string SectionName = "BuildRoadmapWorkspaceSnapshotWorker";

    [JsonProperty("ScheduleIntervalInMilliseconds")]
    public int ScheduleIntervalInMilliseconds { get; set; }

    [JsonIgnore]
    public TimeSpan ScheduleInterval => TimeSpan.FromMilliseconds(ScheduleIntervalInMilliseconds);
}
