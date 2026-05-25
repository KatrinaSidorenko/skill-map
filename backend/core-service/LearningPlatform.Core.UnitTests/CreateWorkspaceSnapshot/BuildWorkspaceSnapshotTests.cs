using SkillMap.Business.PersonalizedRoadmaps.Common;
using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Business.RoadmapsWorkspace.Common;
using SkillMap.Business.RoadmapsWorkspace.Features.CreateWorkspaceSnapshot;
using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;
using SkillMap.Shared.EventBus;
using LearningPlatform.Core.UnitTests.CreateWorkspaceSnapshot;
using SkillMap.Core.PersonalizedRoadmaps;

namespace LearningPlatform.Core.UnitTests.CreateWorkspaceSnapshot;

public class BuildWorkspaceSnapshotTests
{
    private readonly Mock<IRoadmapBlueprintRepository> _blueprintRepoMock = new();
    private readonly Mock<IRoadmapWorkspaceSnapshotRepository> _snapshotRepoMock = new();
    private readonly Mock<IRoadmapWorkspaceEventRepository> _eventRepoMock = new();
    private readonly Mock<IEventBus> _eventBusMock = new();
    private readonly Mock<ILogger<BuildWorkspaceSnapshotHandler>> _loggerMock = new();
    private readonly BuildWorkspaceSnapshotHandler _handler;

    public BuildWorkspaceSnapshotTests()
    {
        _handler = new BuildWorkspaceSnapshotHandler(
            _blueprintRepoMock.Object,
            _snapshotRepoMock.Object,
            _eventRepoMock.Object,
            _eventBusMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Given_LatestSnapshotAndEvents_When_BuildSnapshot_Then_CreatesNewSnapshot()
    {
        // Arrange
        var ct = CancellationToken.None;
        var (workspaceId, roadmapId) = BuildWorkspaceSnapshotFakers.CreateRoadmapIdentifiers();
        var existingSnapshot = await CreateSnapshotWithRoadmapAsync(workspaceId, roadmapId, ct);
        var pendingEvents = BuildWorkspaceSnapshotFakers.FakeWorkspaceEvents(workspaceId, existingSnapshot.Version, 3);

        _snapshotRepoMock
            .Setup(r => r.GetLatestSnapshot(workspaceId, ct))
            .ReturnsAsync(existingSnapshot);
        _eventRepoMock
            .Setup(r => r.GetEventsAfter(workspaceId, existingSnapshot.Version, ct))
            .ReturnsAsync(pendingEvents);
        var command = new BuildWorkspaceSnapshotCommand(workspaceId, roadmapId);

        // Act
        var newSnapshotId = await _handler.Handle(command, ct);

        // Assert
        _snapshotRepoMock.Verify(r => r.AddAsync(It.IsAny<RoadmapWorkspaceSnapshot>(), ct), Times.Once);
        _snapshotRepoMock.Verify(r => r.SaveChangesAsync(ct), Times.Once);
        newSnapshotId.Should().NotBe(existingSnapshot.Id);
    }

    private static async Task<RoadmapWorkspaceSnapshot> CreateSnapshotWithRoadmapAsync(
        long workspaceId, string roadmapId, CancellationToken ct)
    {
        var snapshot = new RoadmapWorkspaceSnapshot(workspaceId);
        snapshot.SetVersion(RoadmapWorkspaceConstants.InitialVersion);
        await snapshot.SetRoadmapSnapshot(BuildWorkspaceSnapshotFakers.FakeRoadmapSnapshot(roadmapId), ct);
        return snapshot;
    }
}
