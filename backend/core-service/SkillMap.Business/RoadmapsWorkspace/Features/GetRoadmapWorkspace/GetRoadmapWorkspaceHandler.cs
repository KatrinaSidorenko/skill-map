using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalizedRoadmaps.Common;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;
using SkillMap.Shared.Results;

namespace SkillMap.Business.PersonalizedRoadmaps.Features.GetPersonalizedRoadmap;

[UsedImplicitly]
internal sealed class GetRoadmapWorkspaceHandler(
    IRepository<RoadmapWorkspace> repository, IRepository<RoadmapWorkspaceEvent> eventsRepository) : IRequestHandler<GetRoadmapWorkspaceQuery, RoadmapWorkspaceDto>
{
    public async Task<RoadmapWorkspaceDto> Handle(GetRoadmapWorkspaceQuery request, CancellationToken cancellationToken)
    {
        var workspace = await repository.GetFirstOrDefaultAsync(w => w.Id == request.WorkspaceId, cancellationToken)
            ?? throw new ResourceNotFoundException(nameof(RoadmapWorkspace), request.WorkspaceId.ToString());
        var latestSnapshot = workspace.Snapshots.OrderByDescending(s => s.CreatedAt).FirstOrDefault()
            ?? throw new ResourceNotFoundException(nameof(RoadmapWorkspaceSnapshot), $"No snapshots found for workspace {request.WorkspaceId}");

        var snapshotContent = await latestSnapshot.GetRoadmapSnapshot(cancellationToken);
        var events = await eventsRepository.GetAllAsync(
            filter: e => e.RoadmapWorkspaceId == request.WorkspaceId && e.Version > latestSnapshot.Version,
            ct: cancellationToken);
        var eventsList = events.ToList();

        var targetSnapshotContent = snapshotContent;
        if (eventsList.Count > 0)
        {
            targetSnapshotContent = await snapshotContent.ApplyEventsToSnapshot(eventsList, cancellationToken);
        }
        
        return RoadmapWorkspaceDto.Create(workspace.Id, targetSnapshotContent, latestSnapshot.Version);
    }
}