using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace.Events;

namespace SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;
public class RoadmapSnapshotAggregator
{
    private readonly string _snapshotId;
    private readonly List<LearningItemSnapshot> _items;
    private readonly List<LearningItemsConnectionSnapshot> _connections;

    public RoadmapSnapshotAggregator(RoadmapSnapshot currentSnapshot)
    {
        _snapshotId = currentSnapshot.Id;
        _items = currentSnapshot.LearningItems?.ToList() ?? [];
        _connections = currentSnapshot.LearningItemsConnections?.ToList() ?? [];
    }

    public void Apply(IWorkspaceEvent @event)
    {
        switch (@event)
        {
            case LearningItemCreatedEvent e:
                Enum.TryParse<LearningStatus>(e.Status, true, out var initialStatus);
                _items.Add(new LearningItemSnapshot(e.Id, e.Title, e.Description, e.Type, initialStatus));
                break;

            case LearningItemUpdatedEvent e:
                var index = _items.FindIndex(i => i.Id == e.Id);
                if (index >= 0)
                {
                    var existingItem = _items[index];

                    // Only update status if it's provided and valid
                    var newStatus = existingItem.Status;
                    if (!string.IsNullOrEmpty(e.Status) && Enum.TryParse<LearningStatus>(e.Status, true, out var parsedStatus))
                    {
                        newStatus = parsedStatus;
                    }

                    // Replace the old record with a new one containing updated values
                    // Coalescing (??) ensures we only overwrite if the event provided a new value
                    _items[index] = existingItem with
                    {
                        Title = e.Title ?? existingItem.Title,
                        Description = e.Description ?? existingItem.Description,
                        Status = newStatus,
                        Type = e.Type ?? existingItem.Type,
                    };
                }
                break;

            case LearningItemDeletedEvent e:
                _items.RemoveAll(i => i.Id == e.Id);
                _connections.RemoveAll(c => c.FromId == e.Id || c.ToId == e.Id);
                break;

            case LearningItemConnectionCreatedEvent e:
                _connections.Add(new LearningItemsConnectionSnapshot(e.Id, e.Source, e.Target));
                break;

            case LearningItemConnectionDeletedEvent e:
                _connections.RemoveAll(c => c.Id == e.Id);
                break;
        }
    }

    public RoadmapSnapshot Build()
    {
        return new RoadmapSnapshot
        {
            Id = _snapshotId,
            LearningItems = _items.DistinctBy(i => i.Id).ToList(),
            LearningItemsConnections = _connections.DistinctBy(c => c.Id).ToList(),
        };
    }
}