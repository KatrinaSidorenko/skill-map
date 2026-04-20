
using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapAssessments.Common;
using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapAssessments;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Results;

namespace SkillMap.Business.RoadmapAssessments.Features.GetRoadmapStateSuggestions;

[UsedImplicitly]
internal class GetRoadmapStateSuggestionsHandler(
    IRepository<RoadmapAssessment> assessmentRepository,
    IRepository<AssessmentAttempt> attemptRepository,
    IRoadmapWorkspaceEditor workspaceEditor) : IRequestHandler<GetRoadmapStateSuggestionsQuery, RoadmapStateSuggestionsDto>
{
    public async Task<RoadmapStateSuggestionsDto> Handle(GetRoadmapStateSuggestionsQuery request, CancellationToken cancellationToken)
    {
        var attempt = await attemptRepository.GetByIdAsync(request.AttemptId, cancellationToken)
            ?? throw new ResourceNotFoundException(nameof(AssessmentAttempt), $"Attempt with id {request.AttemptId} not found.");
        var assessment = await assessmentRepository.GetByIdAsync(attempt.AssessmentId, cancellationToken)
            ?? throw new ResourceNotFoundException(nameof(RoadmapAssessment), $"Assessment with id {attempt.AssessmentId} not found.");
        var workspaceSnapshot = await workspaceEditor.GetActualRoadmapSnapshot(assessment.RoadmapWorkspaceId, cancellationToken)
            ?? throw new ResourceNotFoundException(nameof(RoadmapWorkspace), $"Roadmap workspace with id {assessment.RoadmapWorkspaceId} not found.");

        var attemptContent = await attempt.GetAssessmentAttemptResult(cancellationToken);
        var actualPropagatedWorkspaceStatuses = LearningRoadmapStatusesPropagation.PropagateLearningItemStatuses(workspaceSnapshot);
        var suggestions = CalculateSuggestionStatuses(actualPropagatedWorkspaceStatuses, attemptContent);
        var validatedSuggestions = LearningRoadmapStatusesPropagation.GetValidatedAssessmentSuggestions(actualPropagatedWorkspaceStatuses, suggestions, workspaceSnapshot.LearningItemsConnections);
        var differences = CalculateDifferencesBetweenSnapshots(workspaceSnapshot, validatedSuggestions);
        var itemsConnections = workspaceSnapshot.LearningItemsConnections
            .GroupBy(c => c.FromId)
            .ToDictionary(g => g.Key, g => g.Select(c => c.ToId).ToList());
        var learningItemsDict = workspaceSnapshot.LearningItems.ToDictionary(li => li.Id);
        var topicToSubtopicConnections = workspaceSnapshot.LearningItems
            .Where(li => li.Type == LearningItemType.Topic)
            .ToDictionary(
                li => li.Id,
                li => (itemsConnections.GetOrDefault(li.Id) ?? []).Where(i => learningItemsDict.GetOrDefault(i)?.Type == LearningItemType.SubTopic).ToList());
        return new RoadmapStateSuggestionsDto(differences, topicToSubtopicConnections);
    }

    private List<LearningItemSuggestionDto> CalculateDifferencesBetweenSnapshots(RoadmapSnapshot actualSnapshot, List<LeaningItemAssessment> leaningItemsWithAppliedSuggestions)
    {
        var result = new List<LearningItemSuggestionDto>();
        var actualStatusesDict = actualSnapshot.LearningItems.ToDictionary(li => li.Id, li => li.Status);
        foreach (var learningItem in leaningItemsWithAppliedSuggestions)
        {
            var actualStatus = actualStatusesDict.GetOrDefault(learningItem.Id);
            if (actualStatus == LearningStatus.Skip) continue;

            var suggestedStatus = learningItem.GetLearningStatus();
            if (actualStatus != suggestedStatus)
            {
                result.Add(new LearningItemSuggestionDto(
                    Id: learningItem.Id,
                    Title: learningItem.Title,
                    Type: learningItem.Type,
                    ActualStatus: actualStatus.ToStatusString(),
                    SuggestedStatus: suggestedStatus.ToStatusString()));
            }
        }
        return result;
    }

    private List<LearningItemSuggestion> CalculateSuggestionStatuses(List<LeaningItemAssessment> leaningItems, AssessmentAttemptContent attemptContent)
    {
        var result = new List<LearningItemSuggestion>();
        foreach (var learningItem in leaningItems)
        {
            var actualStatus = learningItem.Status;
            var assumedStatus = learningItem.Assumption;
            var assessmentStatus = GetAssessmentStatus(learningItem.Id, attemptContent);
            var suggestedStatus = RoadmapAssessmentStateHelper.SuggestStatus(actualStatus, assumedStatus, assessmentStatus);
            result.Add(new LearningItemSuggestion(
                Id: learningItem.Id,
                Status: actualStatus,
                Assumption: assumedStatus,
                AssessmentStatus: assessmentStatus,
                SuggestedStatus: suggestedStatus));
        }
        return result;
    }
    private AssessmentStatus GetAssessmentStatus(string learningItemId, AssessmentAttemptContent attemptContent)
    {
        var testContentForItem = attemptContent.LearningItemsAnalysis.GetOrDefault(learningItemId);
        if (testContentForItem == null) return AssessmentStatus.NotTested;

        var achievedPoints = testContentForItem.AchievedPoints;
        var possiblePoints = testContentForItem.TotalPossiblePoints;
        if (achievedPoints == 0) return AssessmentStatus.Failed;
        if (achievedPoints == possiblePoints) return AssessmentStatus.Passed;

        return AssessmentStatus.Failed;
    }
}
