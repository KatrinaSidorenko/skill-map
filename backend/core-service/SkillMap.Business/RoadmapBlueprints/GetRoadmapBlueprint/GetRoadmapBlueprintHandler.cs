using JetBrains.Annotations;

using LearningPlatform.Roadmap.Business.Contracts;

using MediatR;

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
            throw new KeyNotFoundException($"Failed to get roadmap: {result.Message}");
        }

        return RoadmapBlueprintDto.Create(result.Data);
    }
}
