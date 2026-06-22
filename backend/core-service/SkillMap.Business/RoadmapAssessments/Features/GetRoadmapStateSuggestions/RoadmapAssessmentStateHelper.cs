using SkillMap.Business.RoadmapAssessments.Common;
using SkillMap.Core.Constants;

namespace SkillMap.Business.RoadmapAssessments.Features.GetRoadmapStateSuggestions;

internal enum AssessmentStatus
{
    Failed = 0,
    Passed = 1,
    NotTested = 2,
}

public static class RoadmapAssessmentStateHelper
{
    internal static LearningStatus SuggestStatus(
        LearningStatus currentStatus,
        AssessmentAssumption? assumption,
        AssessmentStatus testStatus)
    {
        return (currentStatus, assumption, testStatus) switch
        {
            (LearningStatus.Skip, _, _) => LearningStatus.Skip,
            (_, _, AssessmentStatus.Passed) => LearningStatus.Completed,

            (LearningStatus.NotStarted, _, AssessmentStatus.NotTested) => LearningStatus.NotStarted,
            (LearningStatus.InProgress, _, AssessmentStatus.NotTested) => LearningStatus.InProgress,
            (LearningStatus.Completed, _, AssessmentStatus.NotTested) => LearningStatus.Completed,
            (LearningStatus.Repeat, _, AssessmentStatus.NotTested) => LearningStatus.Repeat,
            (LearningStatus.Upcoming, _, AssessmentStatus.NotTested) => LearningStatus.Upcoming,

            (LearningStatus.Completed, _, AssessmentStatus.Failed) => LearningStatus.Repeat,
            (LearningStatus.Repeat, _, AssessmentStatus.Failed) => LearningStatus.Repeat,

            (LearningStatus.NotStarted, AssessmentAssumption.AssumedCompleted, AssessmentStatus.Failed) => LearningStatus.InProgress,
            (LearningStatus.NotStarted, AssessmentAssumption.AssumedInProgress, AssessmentStatus.Failed) => LearningStatus.InProgress,
            (LearningStatus.NotStarted, _, AssessmentStatus.Failed) => LearningStatus.Upcoming,


            (LearningStatus.InProgress, _, AssessmentStatus.Failed) => LearningStatus.InProgress,
            (LearningStatus.Upcoming, _, AssessmentStatus.Failed) => LearningStatus.InProgress,

            (_, _, AssessmentStatus.Failed) => currentStatus == LearningStatus.Completed
                                                ? LearningStatus.Repeat
                                                : LearningStatus.InProgress,
            _ => currentStatus
        };
    }
}