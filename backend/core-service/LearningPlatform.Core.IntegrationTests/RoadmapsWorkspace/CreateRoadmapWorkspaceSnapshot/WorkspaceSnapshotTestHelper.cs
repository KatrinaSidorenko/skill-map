using SkillMap.Business.RoadmapsWorkspace.Common;

namespace LearningPlatform.Core.IntegrationTests.RoadmapsWorkspace.CreateRoadmapWorkspaceSnapshot;

internal static class WorkspaceSnapshotTestHelper
{
    private static readonly Random _random = Random.Shared;
    public static int RandomEventCountBelowSnapshotInterval(int min = 2, int? max = null)
    {
        var ceiling = (max ?? RoadmapWorkspaceConstants.SnapshotCreationInterval - 1);
        return _random.Next(min, ceiling + 1);
    }
    public static int ExpectedLastVersion(int eventCount, int baseVersion = RoadmapWorkspaceConstants.InitialVersion) => baseVersion + eventCount;
}
