using SkillMap.Core.RoadmapsWorkspace.Events;

namespace SkillMap.Business.RoadmapsWorkspace.Features.GetWorkspaceEventsStatus;

/// <summary>DTO returned for each workspace event the client polled for.</summary>
public record WorkspaceEventStatusDto(
    string IdempotencyKey,
    string Status,
    string? RejectionReason)
{
    public static WorkspaceEventStatusDto FromEvent(Core.PersonalizedRoadmaps.RoadmapWorkspaceEvent e)
 => new(
        IdempotencyKey: e.IdempotencyKey,
        Status: e.EventStatus.ToString().ToLower(),
        RejectionReason: e.EventStatus == WorkspaceEventStatus.Rejected
        ? (e.RejectionReason ?? WorkspaceEventRejectionReason.CycleDetected)
        : null);
}
