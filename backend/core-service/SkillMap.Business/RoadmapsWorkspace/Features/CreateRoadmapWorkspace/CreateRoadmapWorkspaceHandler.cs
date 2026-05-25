using JetBrains.Annotations;

using LearningPlatform.Roadmap.Business.Contracts;

using MediatR;

using Microsoft.Extensions.Logging;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapsWorkspace.Features.CreateWorkspaceSnapshot.Blueprint;
using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Shared.EventBus;
using SkillMap.Shared.Models;

namespace SkillMap.Business.RoadmapsWorkspace.Features.CreateRoadmapWorkspace;

[UsedImplicitly]
internal sealed class CreateRoadmapWorkspaceHandler(
    IRepository<RoadmapWorkspace> repository, 
    ILogger<CreateRoadmapWorkspaceHandler> logger,
    IMediator mediator, 
    IRoadmapBlueprintRepository blueprintRepository) : IRequestHandler<CreateRoadmapWorkspaceCommand, long>
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

        try
        {
            roadmapWorkspace = new RoadmapWorkspace(request.UserId, request.RoadmapId, null);
            var blueprintResult = await blueprintRepository.GetPlainRoadmapsByIds([request.RoadmapId], DefaultParams.SearchingParams, cancellationToken);
            if (blueprintResult.IsFailed) throw new InvalidOperationException($"Failed to get roadmap blueprint for roadmap id {request.RoadmapId}. Error: {blueprintResult.Message}");

            var blueprint = blueprintResult.Data.Result.Single();
            roadmapWorkspace.SetMetadata(blueprint.Title, blueprint.Description, blueprint.ImageUrl);

            await repository.AddAsync(roadmapWorkspace, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var command = new BuildBlueprintWorkspaceSnapshotCommand(roadmapWorkspace.Id, roadmapWorkspace.RoadmapId);
            await mediator.Send(command, cancellationToken);
        }
        catch (Exception ex)
        {
            if (roadmapWorkspace is not null)
            {
                await repository.DeleteAsync(roadmapWorkspace.Id, cancellationToken);
                await repository.SaveChangesAsync(cancellationToken);
            }
          
            logger.LogError(ex, "An error occurred while creating roadmap workspace for user {UserId} and roadmap {RoadmapId}", request.UserId, request.RoadmapId);
            throw;
        }
       

        return roadmapWorkspace.Id;
    }
}