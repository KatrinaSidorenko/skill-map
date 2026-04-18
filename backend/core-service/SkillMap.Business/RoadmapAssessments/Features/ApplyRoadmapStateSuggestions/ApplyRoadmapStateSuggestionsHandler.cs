using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapAssessments;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Shared.EventBus;
using SkillMap.Shared.Results;

namespace SkillMap.Business.RoadmapAssessments.Features.ApplyRoadmapStateSuggestions;

[UsedImplicitly]
internal sealed class ApplyRoadmapStateSuggestionsHandler(
    IRepository<AssessmentAttempt> attemptRepository,
    IRoadmapWorkspaceEventRepository eventRepository,
    IEventBus eventBus)
    : IRequestHandler<ApplyRoadmapStateSuggestionsCommand>
{
    public async Task Handle(
        ApplyRoadmapStateSuggestionsCommand request,
        CancellationToken cancellationToken)
    {
        var attempt = await attemptRepository.GetByIdAsync(request.AttemptId, cancellationToken)
            ?? throw new ResourceNotFoundException(nameof(AssessmentAttempt), request.AttemptId.ToString());
        if (request.Items.Count == 0)
            return;

        // todo: one place to handle this logic
        var workspaceId = attempt.RoadmapAssessment.RoadmapWorkspaceId;
        var baseVersion = await eventRepository.GetLastAvailableEventVersion(
            workspaceId, cancellationToken, withIncrement: true);
        var eventsToAdd = new List<RoadmapWorkspaceEvent>(request.Items.Count);
        for (var i = 0; i < request.Items.Count; i++)
        {
            var version = baseVersion + i;
            var workspaceEvent = request.Items[i].ToWorkspaceEvent(workspaceId, version);
            eventsToAdd.Add(workspaceEvent);
        }

        await eventRepository.AddRangeAsync(eventsToAdd, cancellationToken);
        await eventRepository.SaveChangesAsync(cancellationToken);
        var finalVersion = baseVersion + request.Items.Count;
        await eventBus.PublishAsync(new CreateLearningItemProjectionCommand(Guid.NewGuid(), workspaceId, request.Items.Select(i => i.ToProjectionDto()).ToList(), DateTimeOffset.UtcNow), cancellationToken);
        await eventBus.PublishAsync(RoadmapWorkspaceChangedEvent.Create(workspaceId, finalVersion, request.EventType), cancellationToken);
    }
}
