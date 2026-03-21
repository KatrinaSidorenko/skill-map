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
        var roadmapWorkspace = await repository.GetFirstOrDefaultAsync(rw => rw.AuthorId == request.UserId && rw.RoadmapId == request.RoadmapId, cancellationToken);
        if (roadmapWorkspace is not null)
        {
            if (!roadmapWorkspace.IsActive) roadmapWorkspace.Activate();
            await repository.UpdateAsync(roadmapWorkspace, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            return roadmapWorkspace.Id;
        }

        roadmapWorkspace = new RoadmapWorkspace(request.UserId, request.RoadmapId, null);
        var blueprintResult = await blueprintRepository.GetPlainRoadmapsByIds([request.RoadmapId], DefaultParams.SearchingParams, cancellationToken);
        if (blueprintResult.IsFailed) throw new InvalidOperationException($"Failed to get roadmap blueprint for roadmap id {request.RoadmapId}. Error: {blueprintResult.Message}");
       
        var blueprint = blueprintResult.Data.Result.Single();
        roadmapWorkspace.SetMetadata(new RoadmapWorkspaceMetadata(blueprint.Title, blueprint.Description, blueprint.ImageUrl));

        await repository.AddAsync(roadmapWorkspace, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await eventBus.PublishAsync(RoadmapWorkspaceCreatedEvent.Create(roadmapWorkspace.Id, roadmapWorkspace.RoadmapId, false), cancellationToken);
        return roadmapWorkspace.Id;
    }
}
