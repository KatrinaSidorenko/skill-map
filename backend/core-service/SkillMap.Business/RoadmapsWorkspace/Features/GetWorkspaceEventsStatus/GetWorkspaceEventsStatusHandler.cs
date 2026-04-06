using JetBrains.Annotations;

using MediatR;

namespace SkillMap.Business.RoadmapsWorkspace.Features.GetWorkspaceEventsStatus;

[UsedImplicitly]
internal sealed class GetWorkspaceEventsStatusHandler(IRoadmapWorkspaceEventRepository eventRepository)
    : IRequestHandler<GetWorkspaceEventsStatusQuery, List<WorkspaceEventStatusDto>>
{
    public async Task<List<WorkspaceEventStatusDto>> Handle(
           GetWorkspaceEventsStatusQuery request,
           CancellationToken cancellationToken)
    {
        var keys = request.IdempotencyKeys;

        var events = await eventRepository.GetAllAsync(
            filter: e => e.RoadmapWorkspaceId == request.WorkspaceId
            && keys.Contains(e.IdempotencyKey),
                        ct: cancellationToken);

        return events
            .Select(WorkspaceEventStatusDto.FromEvent)
            .ToList();
    }
}