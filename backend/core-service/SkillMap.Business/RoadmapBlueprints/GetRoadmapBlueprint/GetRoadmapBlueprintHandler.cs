using JetBrains.Annotations;

using LearningPlatform.Roadmap.Business.Contracts;

using MediatR;

using SkillMap.Shared.Results;

namespace SkillMap.Business.RoadmapBlueprints.GetRoadmapBlueprint;

[UsedImplicitly]
internal sealed class GetRoadmapBlueprintHandler(IRoadmapBlueprintRepository repository)
    : IRequestHandler<GetRoadmapBlueprintQuery, RoadmapBlueprintDto>
{
    public async Task<RoadmapBlueprintDto> Handle(GetRoadmapBlueprintQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetRoadmapById(request.RoadmapId, cancellationToken);
        if (result.IsFailed)
        {
            throw new ResourceNotFoundException(nameof(RoadmapBlueprintDto), request.RoadmapId.ToString());
        }

        return RoadmapBlueprintDto.Create(result.Data);
    }
}
