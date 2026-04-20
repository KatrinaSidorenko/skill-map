namespace SkillMap.Core.RoadmapsWorkspace.Events;

public enum WorkspaceEventType
{
    Create = 0,
    Delete = 1,
    UpdateTitle = 2,
    UpdateDescription = 3,
    UpdateStatus = 4,
    UpdatePriority = 5,
    UpdateOrder = 6,
    SnapshotUpdate = 7,
    ConnectionCreated = 8,
    LearningItemCreated = 9,
    ConnectionDeleted = 10,
    DeleteItem = 11,
    LearningItemUpdated = 12,
    LearningItemDeleted = 13,
}