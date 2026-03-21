using JetBrains.Annotations;

using LearningPlatform.Roadmap.Business.Contracts;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Shared.EventBus;
using SkillMap.Shared.Models;

namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateRoadmapWorkspace;

[UsedImplicitly]
internal sealed class CreateRoadmapWorkspaceHandler(IRepository<RoadmapWorkspace> repository, IRoadmapBlueprintRepository blueprintRepository, IEventBus eventBus) : IRequestHandler<CreateRoadmapWorkspaceCommand, long>
{
    public async Task<long> Handle(CreateRoadmapWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var roadmapBookmark = new RoadmapWorkspace(request.UserId, request.RoadmapId, null);
        var blueprintResult = await blueprintRepository.GetPlainRoadmapsByIds([request.RoadmapId], DefaultParams.SearchingParams, cancellationToken);
        if (blueprintResult.IsFailed) throw new InvalidOperationException($"Failed to get roadmap blueprint for roadmap id {request.RoadmapId}. Error: {blueprintResult.Message}");
       
        var blueprint = blueprintResult.Data.Result.Single();
        roadmapBookmark.SetMetadata(new RoadmapWorkspaceMetadata(blueprint.Title, blueprint.Description, blueprint.ImageUrl));

        await repository.AddAsync(roadmapBookmark, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await eventBus.PublishAsync(RoadmapWorkspaceCreatedEvent.Create(roadmapBookmark.Id, roadmapBookmark.RoadmapId, false), cancellationToken);
        return roadmapBookmark.Id;
    }
}
