
using LearningPlatform.Core.IntegrationTests.Engine.Configuration;

namespace LearningPlatform.Core.IntegrationTests.RoadmapsWorkspace;
internal class BuildRoadmapWorkspaceSnapshotConfiguration : IOptionsConfiguration
{
    public Dictionary<string, string?> Get() => new Dictionary<string, string?>
    {
        {"BuildRoadmapWorkspaceSnapshotWorker:ScheduleIntervalInMilliseconds", "2000" }
    };
}
