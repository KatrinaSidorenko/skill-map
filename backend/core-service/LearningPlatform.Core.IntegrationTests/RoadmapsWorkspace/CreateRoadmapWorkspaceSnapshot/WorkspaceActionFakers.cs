using Bogus;

using LearningPlatform.Workspace.WebSockets.Contracts;
using LearningPlatform.Workspace.WebSockets.Contracts.Commands;

using SkillMap.Core.Constants;

namespace LearningPlatform.Core.IntegrationTests.RoadmapsWorkspace.CreateRoadmapWorkspaceSnapshot;

internal static class WorkspaceActionFakers
{
    private static readonly Faker _faker = new();

    private static AddLearningItemActionCommand CreateAddItemCommand(int clientVersion) =>
        new(
            Id: _faker.Random.Guid().ToString(),
            Title: _faker.Lorem.Word(),
            Description: _faker.Lorem.Sentence(),
            Status: LearningStatus.NotStarted.ToStatusString(),
            Type: LearningItemType.Topic.ToString(),
            ClientWorkspaceVersion: clientVersion,
            IdempotencyKey: _faker.Random.Guid().ToString());

    private static UpdateLearningItemActionCommand CreateUpdateItemCommand(string itemId, int clientVersion) =>
        new(
            Id: itemId,
            Title: _faker.Lorem.Word(),
            Description: _faker.Lorem.Sentence(),
            Status: LearningStatus.InProgress.ToStatusString(),
            Type: LearningItemType.Topic.ToString(),
            ClientWorkspaceVersion: clientVersion,
            IdempotencyKey: _faker.Random.Guid().ToString());

    private static DeleteLearningItemActionCommand CreateDeleteItemCommand(string itemId, List<string> incidentConnectionIds, int clientVersion) =>
        new(
            Id: itemId,
            IncidentConnectionIds: incidentConnectionIds,
            ClientWorkspaceVersion: clientVersion,
            IdempotencyKey: _faker.Random.Guid().ToString());

    private static CreateLearningItemConnectionActionCommand CreateConnectionCommand(string sourceId, string targetId, int clientVersion) =>
       new(
            Id: _faker.Random.Guid().ToString(),
            Source: sourceId,
            Target: targetId,
            ClientWorkspaceVersion: clientVersion,
            IdempotencyKey: _faker.Random.Guid().ToString());

    private static DeleteLearningItemConnectionActionCommand CreateDeleteConnectionCommand(string connectionId, int clientVersion) =>
        new(
            Id: connectionId,
            ClientWorkspaceVersion: clientVersion,
            IdempotencyKey: _faker.Random.Guid().ToString());

    public static WorkspaceAction FakeAddLearningItemAction(long workspaceId, int clientVersion = 0)
    {
        var payload = CreateAddItemCommand(clientVersion);
        return new WorkspaceAction(workspaceId, WorkspaceActionType.CreateLearningItem, payload);
    }

    public static WorkspaceAction FakeUpdateLearningItemAction(long workspaceId, string itemId, int clientVersion)
    {
        var payload = CreateUpdateItemCommand(itemId, clientVersion);
        return new WorkspaceAction(workspaceId, WorkspaceActionType.UpdateLearningItem, payload);
    }

    public static WorkspaceAction FakeDeleteLearningItemAction(long workspaceId, string itemId, List<string> incidentConnectionIds, int clientVersion)
    {
        var payload = CreateDeleteItemCommand(itemId, incidentConnectionIds, clientVersion);
        return new WorkspaceAction(workspaceId, WorkspaceActionType.DeleteLearningItem, payload);
    }

    public static WorkspaceAction FakeCreateConnectionAction(long workspaceId, string sourceId, string targetId, int clientVersion)
    {
        var payload = CreateConnectionCommand(sourceId, targetId, clientVersion);
        return new WorkspaceAction(workspaceId, WorkspaceActionType.CreateConnection, payload);
    }

    public static WorkspaceAction FakeDeleteConnectionAction(long workspaceId, string connectionId, int clientVersion)
    {
        var payload = CreateDeleteConnectionCommand(connectionId, clientVersion);
        return new WorkspaceAction(workspaceId, WorkspaceActionType.DeleteConnection, payload);
    }

    public static IReadOnlyList<WorkspaceAction> FakeAddLearningItemActions(long workspaceId, int count, int startClientVersion = 0)
        => Enumerable.Range(0, count)
      .Select(i => FakeAddLearningItemAction(workspaceId, startClientVersion + i))
   .ToList();

    public static IReadOnlyList<WorkspaceAction> FakeMixedActionSequence(long workspaceId, int startClientVersion = 0)
    {
        int v = startClientVersion;

        var addA = FakeAddLearningItemAction(workspaceId, v++);
        var addB = FakeAddLearningItemAction(workspaceId, v++);

        var itemAId = ((AddLearningItemActionCommand)addA.Payload).Id;
        var itemBId = ((AddLearningItemActionCommand)addB.Payload).Id;

        var connectionId = _faker.Random.Guid().ToString();
        var createConn = FakeCreateConnectionAction(workspaceId, itemAId, itemBId, v++);
        var updateA = FakeUpdateLearningItemAction(workspaceId, itemAId, v++);
        var deleteConn = FakeDeleteConnectionAction(workspaceId, connectionId, v++);
        var deleteB = FakeDeleteLearningItemAction(workspaceId, itemBId, incidentConnectionIds: [], v++);

        return [addA, addB, createConn, updateA, deleteConn, deleteB];
    }
}
