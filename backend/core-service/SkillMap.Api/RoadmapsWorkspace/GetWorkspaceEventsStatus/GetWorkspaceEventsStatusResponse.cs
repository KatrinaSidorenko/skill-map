using Newtonsoft.Json;

using SkillMap.Business.RoadmapsWorkspace.Features.GetWorkspaceEventsStatus;

namespace SkillMap.Api.RoadmapsWorkspace.GetWorkspaceEventsStatus;

public class GetWorkspaceEventsStatusResponse
{
    [JsonProperty("events")]
    public List<WorkspaceEventStatusResponseItem> Events { get; set; }

    public static GetWorkspaceEventsStatusResponse Create(List<WorkspaceEventStatusDto> dtos)
        => new()
        {
            Events = dtos.Select(WorkspaceEventStatusResponseItem.Create).ToList()
        };
}

public class WorkspaceEventStatusResponseItem
{
    [JsonProperty("idempotencyKey")]
    public string IdempotencyKey { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("rejectionReason")]
    public string? RejectionReason { get; set; }

    public static WorkspaceEventStatusResponseItem Create(WorkspaceEventStatusDto dto)
        => new()
        {
            IdempotencyKey = dto.IdempotencyKey,
            Status = dto.Status,
            RejectionReason = dto.RejectionReason
        };
}