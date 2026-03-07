using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Shared.Gzip;
using SkillMap.Shared.Results;

namespace SkillMap.Business.PersonalizedRoadmaps.GetPersonalizedRoadmap;

[UsedImplicitly]
internal sealed class GetPersonalizedRoadmapHandler(IRepository<PersonalizedRoadmapSnapshot> repository) : IRequestHandler<GetPersonalizedRoadmapQuery, PersonalizedRoadmapDto>
{
    // when user fork the roadmap should be raised an event for snapshot
    public async Task<PersonalizedRoadmapDto> Handle(GetPersonalizedRoadmapQuery request, CancellationToken cancellationToken)
    {
        // get the latest snapshot of the roadmap
        // for now we assume that version in WAL is same as the version in snapshot, we will update it later when we have WAL implemented
        //throw new NotImplementedException();
        var snapshot = await repository.GetFirstOrDefaultAsync(rs => rs.UserRoadmapId == request.UserRoadmapId, cancellationToken)
            ?? throw new ResourceNotFoundException(nameof(PersonalizedRoadmapSnapshot), request.UserRoadmapId.ToString());

        // some bullshit with versions stuff

        var roadmapSnapshot = await snapshot.GetPersonalizedRoadmapSnapshot(cancellationToken)
            ?? throw new ResourceNotFoundException(nameof(RoadmapSnapshot), request.UserRoadmapId.ToString());
        return PersonalizedRoadmapDto.Create(roadmapSnapshot);
    }
}

internal static class PersonalizedRoadmapSnapshotExtensions
{
    public static async Task<RoadmapSnapshot> GetPersonalizedRoadmapSnapshot(this PersonalizedRoadmapSnapshot snapshot, CancellationToken ct)
        => await snapshot.Content?.InGzipJsonObjectUtf8<RoadmapSnapshot>(ct);
}
