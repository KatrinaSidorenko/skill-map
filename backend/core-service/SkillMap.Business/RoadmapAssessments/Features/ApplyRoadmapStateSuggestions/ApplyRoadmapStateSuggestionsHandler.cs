using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Business.RoadmapsWorkspace.IntegrationEvents;
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

        var workspaceId = attempt.RoadmapAssessment.RoadmapWorkspaceId;
        var baseVersion = await eventRepository.GetLastAvailableEventVersion(
            workspaceId, cancellationToken, withIncrement: true);
        for (var i = 0; i < request.Items.Count; i++)
        {
            var version = baseVersion + i;
            var workspaceEvent = request.Items[i].ToWorkspaceEvent(workspaceId, version);
            await eventRepository.AddAsync(workspaceEvent, cancellationToken);
        }

        await eventRepository.SaveChangesAsync(cancellationToken);
        var finalVersion = baseVersion + request.Items.Count - 1;
        await eventBus.PublishAsync(RoadmapWorkspaceChangedEvent.Create(workspaceId, finalVersion, request.EventType), cancellationToken);
    }
}
