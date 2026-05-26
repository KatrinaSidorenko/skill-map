using Bogus;

using SkillMap.Business.RoadmapsWorkspace.Common;
using SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.AddLearningItem;
using SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.CreateLearningItemConnection;
using SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.DeleteLearningItem;
using SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.DeleteLearningItemConnection;
using SkillMap.Business.RoadmapsWorkspace.Features.WorkspaceEvents.UpdateLearningItem;
using SkillMap.Core.Constants;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;

namespace LearningPlatform.Core.UnitTests.CreateWorkspaceSnapshot;

internal static class BuildWorkspaceSnapshotFakers
{
    public static (long WorkspaceId, string RoadmapId) CreateRoadmapIdentifiers()
    {
        var workspaceId = new Faker().Random.Long(1, long.MaxValue);
        var roadmapId = new Faker().Random.Guid().ToString();
        return (workspaceId, roadmapId);
    }
    private static Faker<LearningItemSnapshot> LearningItemFaker() =>
        new Faker<LearningItemSnapshot>()
            .CustomInstantiator(f => new LearningItemSnapshot(
                Id: f.Random.Guid().ToString(),
                Title: f.Lorem.Word(),
                Description: f.Lorem.Sentence(),
                Type: LearningItemType.Topic,
                Status: LearningStatus.NotStarted
            ));

    private static Faker<LearningItemsConnectionSnapshot> ConnectionFaker(string fromId, string toId) =>
        new Faker<LearningItemsConnectionSnapshot>()
            .CustomInstantiator(f => new LearningItemsConnectionSnapshot(
                Id: f.Random.Guid().ToString(),
                FromId: fromId,
                ToId: toId
            ));

    public static LearningItemSnapshot FakeLearningItem() =>
        LearningItemFaker().Generate();

    public static LearningItemsConnectionSnapshot FakeLearningItemConnection(string fromId, string toId) =>
        ConnectionFaker(fromId, toId).Generate();

    public static RoadmapSnapshot FakeRoadmapSnapshot(string roadmapId)
    {
        var item1 = FakeLearningItem();
        var item2 = FakeLearningItem();

        return new RoadmapSnapshot
        {
            Id = roadmapId,
            LearningItems = [item1, item2],
            LearningItemsConnections = [FakeLearningItemConnection(item1.Id, item2.Id)]
        };
    }

    public static AddLearningItemCommand FakeAddLearningItemCommand(long workspaceId, int baseVersion = RoadmapWorkspaceConstants.InitialVersion) =>
        new Faker<AddLearningItemCommand>()
            .CustomInstantiator(f => new AddLearningItemCommand(
                WorkspaceId: workspaceId,
                Id: f.Random.Guid().ToString(),
                Title: f.Lorem.Word(),
                Description: f.Lorem.Sentence(),
                Status: LearningStatus.NotStarted.ToStatusString(),
                Type: LearningItemType.Topic,
                BaseVersion: baseVersion,
                IdempotencyKey: f.Random.Guid().ToString()
            ))
            .Generate();

    public static UpdateLearningItemCommand FakeUpdateLearningItemCommand(long workspaceId, string itemId, int baseVersion) =>
        new Faker<UpdateLearningItemCommand>()
            .CustomInstantiator(f => new UpdateLearningItemCommand(
                WorkspaceId: workspaceId,
                Id: itemId,
                Title: f.Lorem.Word(),
                Description: f.Lorem.Sentence(),
                Status: LearningStatus.InProgress.ToStatusString(),
                Type: LearningItemType.Topic,
                BaseVersion: baseVersion,
                IdempotencyKey: f.Random.Guid().ToString()
            ))
            .Generate();

    public static DeleteLearningItemCommand FakeDeleteLearningItemCommand(long workspaceId, string itemId, List<string> connectionIds, int baseVersion) =>
        new Faker<DeleteLearningItemCommand>()
            .CustomInstantiator(f => new DeleteLearningItemCommand(
                WorkspaceId: workspaceId,
                Id: itemId,
                IncidentConnectionIds: connectionIds,
                BaseVersion: baseVersion,
                IdempotencyKey: f.Random.Guid().ToString()
            ))
            .Generate();

    public static CreateLearningItemConnectionCommand FakeCreateConnectionCommand(long workspaceId, string fromId, string toId, int baseVersion) =>
        new Faker<CreateLearningItemConnectionCommand>()
            .CustomInstantiator(f => new CreateLearningItemConnectionCommand(
                WorkspaceId: workspaceId,
                Id: f.Random.Guid().ToString(),
                Source: fromId,
                Target: toId,
                BaseVersion: baseVersion,
                IdempotencyKey: f.Random.Guid().ToString()
            ))
            .Generate();

    public static DeleteLearningItemConnectionCommand FakeDeleteConnectionCommand(long workspaceId, string connectionId, int baseVersion) =>
        new Faker<DeleteLearningItemConnectionCommand>()
            .CustomInstantiator(f => new DeleteLearningItemConnectionCommand(
                WorkspaceId: workspaceId,
                Id: connectionId,
                BaseVersion: baseVersion,
                IdempotencyKey: f.Random.Guid().ToString()
            ))
            .Generate();

    public static List<RoadmapWorkspaceEvent> FakeWorkspaceEvents(long workspaceId, int startVersion, int numberOfEvents)
    {
        var result = new List<RoadmapWorkspaceEvent>();
        for (int i = 0; i < numberOfEvents; i++)
        {
            result.Add(FakeAddLearningItemCommand(workspaceId, startVersion).ToRoadmapWorkspaceEvent());
        }
        return result;
    }
}
