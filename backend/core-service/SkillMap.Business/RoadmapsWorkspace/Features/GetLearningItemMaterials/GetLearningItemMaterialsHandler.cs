using JetBrains.Annotations;

using LearningPlatform.Roadmap.Business.Contracts;

using MediatR;

using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Shared.Results;

namespace SkillMap.Business.RoadmapsWorkspace.Features.GetLearningItemMaterials;

[UsedImplicitly]
internal sealed class GetLearningItemMaterialsHandler(
    IRoadmapWorkspaceRepository workspaceRepository,
    IRoadmapBlueprintRepository blueprintRepository) : IRequestHandler<GetLearningItemMaterialsQuery, List<LearningItemMaterialDto>>
{
    public async Task<List<LearningItemMaterialDto>> Handle(GetLearningItemMaterialsQuery request, CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetUserActiveWorkspace(request.WorkspaceId, cancellationToken)
            ?? throw new ResourceNotFoundException(nameof(Core.RoadmapsWorkspace.RoadmapWorkspace), request.WorkspaceId.ToString());

        var roadmapId = workspace.ActualRoadmapId;

        var result = await blueprintRepository.GetLearningItemMaterials(roadmapId, request.LearningItemId, cancellationToken);
        if (result.IsFailed || result.Data is null || result.Data.Count == 0)
            return [];

        return result.Data
            .Select(r => new LearningItemMaterialDto(
                r.Id,
                r.Title,
                r.Link,
                MaterialTypeExtensions.Parse(r.Type)))
            .ToList();
    }
}
