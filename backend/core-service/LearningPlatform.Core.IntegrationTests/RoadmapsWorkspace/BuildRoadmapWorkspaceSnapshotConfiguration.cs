
using LearningPlatform.Core.IntegrationTests.Engine.Configuration;

namespace LearningPlatform.Core.IntegrationTests.RoadmapsWorkspace;
internal class BuildRoadmapWorkspaceSnapshotConfiguration : IOptionsConfiguration
{
    public Dictionary<string, string?> Get() => new Dictionary<string, string?>
    {
        {"BuildRoadmapWorkspaceSnapshotWorkerOptions:ScheduleInterval", "00:00:05" }
    };
}
