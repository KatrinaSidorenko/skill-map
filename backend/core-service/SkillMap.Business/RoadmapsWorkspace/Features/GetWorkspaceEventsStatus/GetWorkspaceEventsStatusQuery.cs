namespace SkillMap.Business.RoadmapsWorkspace.Features.GetWorkspaceEventsStatus;

/// <summary>
/// Returns the status (and optional rejection reason) for a set of workspace events
/// identified by their idempotency keys.
/// </summary>
public record GetWorkspaceEventsStatusQuery(
    long WorkspaceId,
    IReadOnlyList<string> IdempotencyKeys)
    : ICommand<List<WorkspaceEventStatusDto>>;