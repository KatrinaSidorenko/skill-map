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
        _snapshotRepoMock
            .Setup(repo => repo.AddAsync(It.IsAny<RoadmapWorkspaceSnapshot>(), It.IsAny<CancellationToken>()))
            .Callback<RoadmapWorkspaceSnapshot, CancellationToken>((snapshot, ct) =>
            {
                snapshot.Id = existingSnapshot.Id + 1;
            })
            .Returns(Task.FromResult(true));

        var command = new BuildWorkspaceSnapshotCommand(workspaceId, roadmapId);

        // Act
        var newSnapshotId = await _handler.Handle(command, ct);

        // Assert
        _snapshotRepoMock.Verify(r => r.AddAsync(It.IsAny<RoadmapWorkspaceSnapshot>(), ct), Times.Once);
        _snapshotRepoMock.Verify(r => r.SaveChangesAsync(ct), Times.Once);
        newSnapshotId.Should().NotBe(existingSnapshot.Id);
    }


    [Fact]
    public async Task Given_SnapshotAndAddItemEvent_When_CreateRoadmapWorkspaceSnapshot_Then_NewSnapshotContainsAddedItem()
    {
        // Arrange
        var ct = CancellationToken.None;
        var (workspaceId, roadmapId) = BuildWorkspaceSnapshotFakers.CreateRoadmapIdentifiers();
        var latestSnapshot = await CreateSnapshotWithRoadmapAsync(workspaceId, roadmapId, ct);
        var snapshotContent = await latestSnapshot.GetRoadmapSnapshot(ct);
        var itemsBefore = snapshotContent.LearningItems.Count;

        var addCmd = BuildWorkspaceSnapshotFakers.FakeAddLearningItemCommand(workspaceId, latestSnapshot.Version);
        var events = new List<RoadmapWorkspaceEvent> { addCmd.ToRoadmapWorkspaceEvent() };

        // Act
        var newSnapshot = await RoadmapWorkspaceSnapshotExtensions.CreateRoadmapWorkspaceSnapshot(workspaceId, latestSnapshot, events, ct);
        var newContent = await newSnapshot.WithNavigationWorkspace(workspaceId).GetRoadmapSnapshot(ct);

        // Assert
        newSnapshot.Version.Should().Be(events.Max(e => e.Version));
        newContent.LearningItems.Should().HaveCount(itemsBefore + 1);
        newContent.LearningItems.Should().Contain(i => i.Id == addCmd.Id);
    }

    [Fact]
    public async Task Given_SnapshotAndMultipleEvents_When_CreateRoadmapWorkspaceSnapshot_Then_VersionIsSetToHighestEventVersion()
    {
        // Arrange
        var ct = CancellationToken.None;
        var (workspaceId, roadmapId) = BuildWorkspaceSnapshotFakers.CreateRoadmapIdentifiers();
        var latestSnapshot = await CreateSnapshotWithRoadmapAsync(workspaceId, roadmapId, ct);
        var baseVersion = latestSnapshot.Version;

        var events = new List<RoadmapWorkspaceEvent>
        {
            BuildWorkspaceSnapshotFakers.FakeAddLearningItemCommand(workspaceId, baseVersion).ToRoadmapWorkspaceEvent(),
            BuildWorkspaceSnapshotFakers.FakeAddLearningItemCommand(workspaceId, baseVersion + 1).ToRoadmapWorkspaceEvent(),
            BuildWorkspaceSnapshotFakers.FakeAddLearningItemCommand(workspaceId, baseVersion + 2).ToRoadmapWorkspaceEvent(),
        };

        var expectedVersion = events.Max(e => e.Version);

        // Act
        var newSnapshot = await RoadmapWorkspaceSnapshotExtensions.CreateRoadmapWorkspaceSnapshot(
            workspaceId, latestSnapshot, events, ct);

        // Assert
        newSnapshot.Version.Should().Be(expectedVersion);
        newSnapshot.RoadmapWorkspaceId.Should().Be(workspaceId);
    }

    [Fact]
    public async Task Given_AddLearningItemEvent_When_ApplyEventsToSnapshot_Then_ItemIsAdded()
    {
        // Arrange
        var ct = CancellationToken.None;
        var (workspaceId, roadmapId) = BuildWorkspaceSnapshotFakers.CreateRoadmapIdentifiers();
        var snapshot = BuildWorkspaceSnapshotFakers.FakeRoadmapSnapshot(roadmapId);
        var countBefore = snapshot.LearningItems.Count;

        var addCmd = BuildWorkspaceSnapshotFakers.FakeAddLearningItemCommand(workspaceId);
        var events = new List<RoadmapWorkspaceEvent> { addCmd.ToRoadmapWorkspaceEvent() };

        // Act
        var result = await snapshot.ApplyEventsToSnapshot(events, ct);

        // Assert
        result.LearningItems.Should().HaveCount(countBefore + 1);
        result.LearningItems.Should().Contain(i => i.Id == addCmd.Id && i.Title == addCmd.Title);
    }

    [Fact]
    public async Task Given_UpdateLearningItemEvent_When_ApplyEventsToSnapshot_Then_ItemIsUpdated()
    {
        // Arrange
        var ct = CancellationToken.None;
        var (workspaceId, roadmapId) = BuildWorkspaceSnapshotFakers.CreateRoadmapIdentifiers();
        var snapshot = BuildWorkspaceSnapshotFakers.FakeRoadmapSnapshot(roadmapId);
        var targetItem = snapshot.LearningItems.First();

        var updateCmd = BuildWorkspaceSnapshotFakers.FakeUpdateLearningItemCommand(workspaceId, targetItem.Id, baseVersion: 1);
        var events = new List<RoadmapWorkspaceEvent> { updateCmd.ToRoadmapWorkspaceEvent() };

        // Act
        var result = await snapshot.ApplyEventsToSnapshot(events, ct);

        // Assert
        var updatedItem = result.LearningItems.Single(i => i.Id == targetItem.Id);
        updatedItem.Title.Should().Be(updateCmd.Title);
        updatedItem.Status.Should().Be(LearningStatus.InProgress);
        result.LearningItems.Should().HaveCount(snapshot.LearningItems.Count);
    }

    [Fact]
    public async Task Given_DeleteLearningItemEvent_When_ApplyEventsToSnapshot_Then_ItemAndIncidentConnectionsAreRemoved()
    {
        // Arrange
        var ct = CancellationToken.None;
        var (workspaceId, roadmapId) = BuildWorkspaceSnapshotFakers.CreateRoadmapIdentifiers();
        var snapshot = BuildWorkspaceSnapshotFakers.FakeRoadmapSnapshot(roadmapId);
        var targetItem = snapshot.LearningItems.First();
        var incidentConnectionIds = snapshot.LearningItemsConnections
            .Where(c => c.FromId == targetItem.Id || c.ToId == targetItem.Id)
            .Select(c => c.Id)
            .ToList();

        var deleteCmd = BuildWorkspaceSnapshotFakers.FakeDeleteLearningItemCommand(
            workspaceId, targetItem.Id, incidentConnectionIds, baseVersion: 1);
        var events = new List<RoadmapWorkspaceEvent> { deleteCmd.ToRoadmapWorkspaceEvent() };

        // Act
        var result = await snapshot.ApplyEventsToSnapshot(events, ct);

        // Assert
        result.LearningItems.Should().NotContain(i => i.Id == targetItem.Id);
        result.LearningItemsConnections.Should().NotContain(c =>
            c.FromId == targetItem.Id || c.ToId == targetItem.Id);
    }

    [Fact]
    public async Task Given_CreateConnectionEvent_When_ApplyEventsToSnapshot_Then_ConnectionIsAdded()
    {
        // Arrange
        var ct = CancellationToken.None;
        var (workspaceId, roadmapId) = BuildWorkspaceSnapshotFakers.CreateRoadmapIdentifiers();
        var snapshot = BuildWorkspaceSnapshotFakers.FakeRoadmapSnapshot(roadmapId);
        var connectionsBefore = snapshot.LearningItemsConnections.Count;

        var item1 = snapshot.LearningItems[0];
        var item2 = snapshot.LearningItems[1];
        var createConnCmd = BuildWorkspaceSnapshotFakers.FakeCreateConnectionCommand(
            workspaceId, item2.Id, item1.Id, baseVersion: 1);
        var events = new List<RoadmapWorkspaceEvent> { createConnCmd.ToRoadmapWorkspaceEvent() };

        // Act
        var result = await snapshot.ApplyEventsToSnapshot(events, ct);

        // Assert
        result.LearningItemsConnections.Should().HaveCount(connectionsBefore + 1);
        result.LearningItemsConnections.Should().Contain(c => c.Id == createConnCmd.Id);
    }

    [Fact]
    public async Task Given_DeleteConnectionEvent_When_ApplyEventsToSnapshot_Then_ConnectionIsRemoved()
    {
        // Arrange
        var ct = CancellationToken.None;
        var (workspaceId, roadmapId) = BuildWorkspaceSnapshotFakers.CreateRoadmapIdentifiers();
        var snapshot = BuildWorkspaceSnapshotFakers.FakeRoadmapSnapshot(roadmapId);
        var targetConnection = snapshot.LearningItemsConnections.First();

        var deleteConnCmd = BuildWorkspaceSnapshotFakers.FakeDeleteConnectionCommand(
            workspaceId, targetConnection.Id, baseVersion: 1);
        var events = new List<RoadmapWorkspaceEvent> { deleteConnCmd.ToRoadmapWorkspaceEvent() };

        // Act
        var result = await snapshot.ApplyEventsToSnapshot(events, ct);

        // Assert
        result.LearningItemsConnections.Should().NotContain(c => c.Id == targetConnection.Id);
        result.LearningItemsConnections.Should().HaveCount(snapshot.LearningItemsConnections.Count - 1);
    }

    [Fact]
    public async Task Given_EventsOutOfOrder_When_ApplyEventsToSnapshot_Then_EventsAreAppliedInVersionOrder()
    {
        // Arrange
        var ct = CancellationToken.None;
        var (workspaceId, roadmapId) = BuildWorkspaceSnapshotFakers.CreateRoadmapIdentifiers();
        var snapshot = BuildWorkspaceSnapshotFakers.FakeRoadmapSnapshot(roadmapId);
        var targetItem = snapshot.LearningItems.First();

        // version 2 updates first, version 1 adds — should be processed in order (add v1, update v2)
        var addCmd = BuildWorkspaceSnapshotFakers.FakeAddLearningItemCommand(workspaceId, baseVersion: 0);     // → version 1
        var updateCmd = BuildWorkspaceSnapshotFakers.FakeUpdateLearningItemCommand(workspaceId, addCmd.Id, baseVersion: 1); // → version 2

        var events = new List<RoadmapWorkspaceEvent>
        {
            updateCmd.ToRoadmapWorkspaceEvent(),   // intentionally submitted out of order
            addCmd.ToRoadmapWorkspaceEvent(),
        };

        // Act
        var result = await snapshot.ApplyEventsToSnapshot(events, ct);

        var resultItem = result.LearningItems.SingleOrDefault(i => i.Id == addCmd.Id);
        resultItem.Should().NotBeNull();
        resultItem!.Title.Should().Be(updateCmd.Title);
    }

    [Fact]
    public async Task Given_EmptyEventsList_When_ApplyEventsToSnapshot_Then_SnapshotIsUnchanged()
    {
        // Arrange
        var ct = CancellationToken.None;
        var snapshot = BuildWorkspaceSnapshotFakers.FakeRoadmapSnapshot(string.Empty);

        // Act
        var result = await snapshot.ApplyEventsToSnapshot([], ct);

        // Assert
        result.LearningItems.Should().BeEquivalentTo(snapshot.LearningItems);
        result.LearningItemsConnections.Should().BeEquivalentTo(snapshot.LearningItemsConnections);
    }

    private static async Task<RoadmapWorkspaceSnapshot> CreateSnapshotWithRoadmapAsync(
        long workspaceId, string roadmapId, CancellationToken ct)
    {
        var snapshot = new RoadmapWorkspaceSnapshot(workspaceId);
        snapshot.SetVersion(RoadmapWorkspaceConstants.InitialVersion);
        await snapshot.SetRoadmapSnapshot(BuildWorkspaceSnapshotFakers.FakeRoadmapSnapshot(roadmapId), ct);
        return snapshot.WithNavigationWorkspace(workspaceId);
    }
}
