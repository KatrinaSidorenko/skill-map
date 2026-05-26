using Bogus;

using LearningPlatform.Workspace.WebSockets.Contracts;
using LearningPlatform.Workspace.WebSockets.Contracts.Commands;

using SkillMap.Core.Constants;

namespace LearningPlatform.Core.IntegrationTests.RoadmapsWorkspace.CreateRoadmapWorkspaceSnapshot;

internal static class WorkspaceActionFakers
{
    private static Faker<AddLearningItemActionCommand> AddItemFaker(int clientVersion) =>
        new Faker<AddLearningItemActionCommand>()
            .RuleFor(x => x.Id, f => f.Random.Guid().ToString())
            .RuleFor(x => x.Title, f => f.Lorem.Word())
            .RuleFor(x => x.Description, f => f.Lorem.Sentence())
            .RuleFor(x => x.Status, _ => LearningStatus.NotStarted.ToStatusString())
            .RuleFor(x => x.Type, _ => LearningItemType.Topic)
            .RuleFor(x => x.ClientWorkspaceVersion, _ => clientVersion)
            .RuleFor(x => x.IdempotencyKey, f => f.Random.Guid().ToString());

    private static Faker<UpdateLearningItemActionCommand> UpdateItemFaker(string itemId, int clientVersion) =>
        new Faker<UpdateLearningItemActionCommand>()
            .RuleFor(x => x.Id, _ => itemId)
            .RuleFor(x => x.Title, f => f.Lorem.Word())
            .RuleFor(x => x.Description, f => f.Lorem.Sentence())
            .RuleFor(x => x.Status, _ => LearningStatus.InProgress.ToStatusString())
            .RuleFor(x => x.Type, _ => LearningItemType.Topic)
            .RuleFor(x => x.ClientWorkspaceVersion, _ => clientVersion)
            .RuleFor(x => x.IdempotencyKey, f => f.Random.Guid().ToString());

    private static Faker<DeleteLearningItemActionCommand> DeleteItemFaker(string itemId, List<string> incidentConnectionIds, int clientVersion) =>
        new Faker<DeleteLearningItemActionCommand>()
            .RuleFor(x => x.Id, _ => itemId)
            .RuleFor(x => x.IncidentConnectionIds, _ => incidentConnectionIds)
            .RuleFor(x => x.ClientWorkspaceVersion, _ => clientVersion)
            .RuleFor(x => x.IdempotencyKey, f => f.Random.Guid().ToString());

    private static Faker<CreateLearningItemConnectionActionCommand> CreateConnectionFaker(string sourceId, string targetId, int clientVersion) =>
        new Faker<CreateLearningItemConnectionActionCommand>()
            .RuleFor(x => x.Id, f => f.Random.Guid().ToString())
            .RuleFor(x => x.Source, _ => sourceId)
            .RuleFor(x => x.Target, _ => targetId)
            .RuleFor(x => x.ClientWorkspaceVersion, _ => clientVersion)
            .RuleFor(x => x.IdempotencyKey, f => f.Random.Guid().ToString());

    private static Faker<DeleteLearningItemConnectionActionCommand> DeleteConnectionFaker(string connectionId, int clientVersion) =>
        new Faker<DeleteLearningItemConnectionActionCommand>()
            .RuleFor(x => x.Id, _ => connectionId)
            .RuleFor(x => x.ClientWorkspaceVersion, _ => clientVersion)
            .RuleFor(x => x.IdempotencyKey, f => f.Random.Guid().ToString());

    public static WorkspaceAction FakeAddLearningItemAction(long workspaceId, int clientVersion = 0)
    {
        var payload = AddItemFaker(clientVersion).Generate();
        return new WorkspaceAction(workspaceId, WorkspaceActionType.CreateLearningItem, payload);
    }

    public static WorkspaceAction FakeUpdateLearningItemAction(long workspaceId, string itemId, int clientVersion)
    {
        var payload = UpdateItemFaker(itemId, clientVersion).Generate();
        return new WorkspaceAction(workspaceId, WorkspaceActionType.UpdateLearningItem, payload);
    }

    public static WorkspaceAction FakeDeleteLearningItemAction(long workspaceId, string itemId, List<string> incidentConnectionIds, int clientVersion)
    {
        var payload = DeleteItemFaker(itemId, incidentConnectionIds, clientVersion).Generate();
        return new WorkspaceAction(workspaceId, WorkspaceActionType.DeleteLearningItem, payload);
    }

    public static WorkspaceAction FakeCreateConnectionAction(long workspaceId, string sourceId, string targetId, int clientVersion)
    {
        var payload = CreateConnectionFaker(sourceId, targetId, clientVersion).Generate();
        return new WorkspaceAction(workspaceId, WorkspaceActionType.CreateConnection, payload);
    }

    public static WorkspaceAction FakeDeleteConnectionAction(long workspaceId, string connectionId, int clientVersion)
    {
        var payload = DeleteConnectionFaker(connectionId, clientVersion).Generate();
        return new WorkspaceAction(workspaceId, WorkspaceActionType.DeleteConnection, payload);
    }

    public static IReadOnlyList<WorkspaceAction> FakeAddLearningItemActions(long workspaceId, int count, int startClientVersion = 0)
        => Enumerable.Range(0, count)
        .Select(i => FakeAddLearningItemAction(workspaceId, startClientVersion + i))
        .ToList();

    public static IReadOnlyList<WorkspaceAction> FakeMixedActionSequence(long workspaceId, int startClientVersion = 0)
    {
        var faker = new Faker();
        int v = startClientVersion;

        var addA = FakeAddLearningItemAction(workspaceId, v++);
        var addB = FakeAddLearningItemAction(workspaceId, v++);

        var itemAId = ((AddLearningItemActionCommand)addA.Payload).Id;
        var itemBId = ((AddLearningItemActionCommand)addB.Payload).Id;

        var connectionId = faker.Random.Guid().ToString();
        var createConn = FakeCreateConnectionAction(workspaceId, itemAId, itemBId, v++);
        var updateA = FakeUpdateLearningItemAction(workspaceId, itemAId, v++);
        var deleteConn = FakeDeleteConnectionAction(workspaceId, connectionId, v++);
        var deleteB = FakeDeleteLearningItemAction(workspaceId, itemBId, incidentConnectionIds: [], v++);

        return [addA, addB, createConn, updateA, deleteConn, deleteB];
    }
}
