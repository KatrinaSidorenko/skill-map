using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;

namespace SkillMap.Business.RoadmapAssessments.Common;

internal enum AssessmentAssumption
{
    AssumedCompleted = 0,
    AssumedInProgress = 1,
}
internal record LeaningItemAssessment(
    string Id,
    string Title,
    string Description,
    string Type,
    LearningStatus Status,
    AssessmentAssumption? Assumption)
    : LearningItemSnapshot(Id, Title, Description, Type, Status)
{
    public static LeaningItemAssessment FromLearningItemSnapshot(LearningItemSnapshot snapshot, AssessmentAssumption? assumption)
    {
        return new LeaningItemAssessment(
            snapshot.Id,
            snapshot.Title,
            snapshot.Description,
            snapshot.Type,
            snapshot.Status,
            assumption);
    }

    public LearningStatus GetLearningStatus()
    {
        return !Assumption.HasValue ? Status
            : Assumption.Value switch
            {
                AssessmentAssumption.AssumedCompleted => LearningStatus.Completed,
                AssessmentAssumption.AssumedInProgress => LearningStatus.InProgress,
                _ => Status
            };
    }
}
internal record LearningItemsConnectionAssessment(
    string Id,
    string FromId,
    string ToId)
    : LearningItemsConnectionSnapshot(Id, FromId, ToId)
{
    public static LearningItemsConnectionAssessment FromLearningItemsConnectionSnapshot(LearningItemsConnectionSnapshot snapshot)
    {
        return new LearningItemsConnectionAssessment(
            snapshot.Id,
            snapshot.FromId,
            snapshot.ToId);
    }
}

internal record LearningItemSuggestion(
    string Id,
    LearningStatus Status,
    AssessmentAssumption? Assumption,
    LearningStatus SuggestedStatus);