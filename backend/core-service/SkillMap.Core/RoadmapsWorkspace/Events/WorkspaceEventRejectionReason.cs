namespace SkillMap.Core.RoadmapsWorkspace.Events;

/// <summary>
/// Well-known string codes for why a workspace event was rejected.
/// </summary>
public static class WorkspaceEventRejectionReason
{
    /// <summary>Applying the connection would create a directed cycle in the workspace graph.</summary>
 public const string CycleDetected = "CYCLE_DETECTED";
}
