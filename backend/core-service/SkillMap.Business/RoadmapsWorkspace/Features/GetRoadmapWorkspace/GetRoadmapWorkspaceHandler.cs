using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.PersonalizedRoadmaps.Common;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;
using SkillMap.Shared.Gzip;
using SkillMap.Shared.Results;

namespace SkillMap.Business.PersonalizedRoadmaps.Features.GetPersonalizedRoadmap;

[UsedImplicitly]
internal sealed class GetRoadmapWorkspaceHandler(IRepository<RoadmapWorkspace> repository) : IRequestHandler<GetRoadmapWorkspaceQuery, RoadmapWorkspaceDto>
{
    // when user fork the roadmap should be raised an event for snapshot
    public async Task<RoadmapWorkspaceDto> Handle(GetRoadmapWorkspaceQuery request, CancellationToken cancellationToken)
    {
        // get the latest snapshot of the roadmap
        // for now we assume that version in WAL is same as the version in snapshot, we will update it later when we have WAL implemented
        //throw new NotImplementedException();
        //var snapshot = await repository.GetFirstOrDefaultAsync(rs => rs.RoadmapWorkspaceId == request.UserRoadmapId, cancellationToken)
        //    ?? throw new ResourceNotFoundException(nameof(RoadmapWorkspaceSnapshot), request.UserRoadmapId.ToString());

        //// some bullshit with versions stuff

        //var roadmapSnapshot = await snapshot.GetRoadmapSnapshot(cancellationToken)
        //    ?? throw new ResourceNotFoundException(nameof(RoadmapSnapshot), request.UserRoadmapId.ToString());

        //// get the WAL log after current version and apply it to the snapshot, for now we will skip this step
        //return RoadmapWorkspaceDto.Create(roadmapSnapshot);
        var workspace = await repository.GetFirstOrDefaultAsync(w => w.Id == request.WorkspaceId, cancellationToken)
            ?? throw new ResourceNotFoundException(nameof(RoadmapWorkspace), request.WorkspaceId.ToString());
        var latestSnapshot = workspace.Snapshots.OrderByDescending(s => s.CreatedAt).FirstOrDefault()
            ?? throw new ResourceNotFoundException(nameof(RoadmapWorkspaceSnapshot), $"No snapshots found for workspace {request.WorkspaceId}");

        var snapshotContent = await latestSnapshot.GetRoadmapSnapshot(cancellationToken);
        return RoadmapWorkspaceDto.Create(workspace.Id, snapshotContent, latestSnapshot.Version);
    }
}


