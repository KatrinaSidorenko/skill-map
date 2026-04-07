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
    private static readonly Dictionary<string, Dictionary<AssessmentStatus, LearningStatus>> _statusesMatrix = new()
    {
        {
            "notstarted_none", new Dictionary<AssessmentStatus, LearningStatus>
            {
                { AssessmentStatus.Passed, LearningStatus.InProgress },
                { AssessmentStatus.Failed, LearningStatus.Upcoming },
                { AssessmentStatus.NotTested, LearningStatus.NotStarted }
            }
        },
        {
            "inprogress_none", new Dictionary<AssessmentStatus, LearningStatus>
            {
                { AssessmentStatus.Passed, LearningStatus.Completed },
                { AssessmentStatus.Failed, LearningStatus.InProgress },
                { AssessmentStatus.NotTested, LearningStatus.InProgress }
            }
        },
        {
            "completed_none", new Dictionary<AssessmentStatus, LearningStatus>
            {
                { AssessmentStatus.Passed, LearningStatus.Completed },
                { AssessmentStatus.Failed, LearningStatus.Repeat }, 
                { AssessmentStatus.NotTested, LearningStatus.Completed }
            }
        },
        {
            "skip_none", new Dictionary<AssessmentStatus, LearningStatus>
            {
                { AssessmentStatus.Passed, LearningStatus.Skip },
                { AssessmentStatus.Failed, LearningStatus.Skip },
                { AssessmentStatus.NotTested, LearningStatus.Skip }
            }
        },
        {
            "repeat_none", new Dictionary<AssessmentStatus, LearningStatus>
            {
                { AssessmentStatus.Passed, LearningStatus.Completed },
                { AssessmentStatus.Failed, LearningStatus.Repeat },
                { AssessmentStatus.NotTested, LearningStatus.Repeat }
            }
        },
        {
            "upcoming_none", new Dictionary<AssessmentStatus, LearningStatus>
            {
                { AssessmentStatus.Passed, LearningStatus.Completed },
                { AssessmentStatus.Failed, LearningStatus.InProgress },
                { AssessmentStatus.NotTested, LearningStatus.Upcoming }
            }
        },


        {
            "notstarted_assumedcompleted", new Dictionary<AssessmentStatus, LearningStatus>
            {
                { AssessmentStatus.Passed, LearningStatus.Completed }, 
                { AssessmentStatus.Failed, LearningStatus.InProgress },
                { AssessmentStatus.NotTested, LearningStatus.NotStarted }
            }
        },
        {
            "inprogress_assumedcompleted", new Dictionary<AssessmentStatus, LearningStatus>
            {
                { AssessmentStatus.Passed, LearningStatus.Completed },
                { AssessmentStatus.Failed, LearningStatus.InProgress },
                { AssessmentStatus.NotTested, LearningStatus.InProgress }
            }
        },

        {
            "notstarted_assumedinprogress", new Dictionary<AssessmentStatus, LearningStatus>
            {
                { AssessmentStatus.Passed, LearningStatus.Completed },
                { AssessmentStatus.Failed, LearningStatus.InProgress },
                { AssessmentStatus.NotTested, LearningStatus.NotStarted }
            }
        }
    };


    internal static LearningStatus SuggestStatus(
        LearningStatus currentStatus,
        AssessmentAssumption? assumption,
        AssessmentStatus testStatus)
    {
        var assumptionKey = assumption?.ToString().ToLowerInvariant() ?? "none";
        var lookupKey = $"{currentStatus.ToString().ToLowerInvariant()}_{assumptionKey}";

        if (_statusesMatrix.TryGetValue(lookupKey, out var testTransitions))
        {
            if (testTransitions.TryGetValue(testStatus, out var newStatus))
            {
                return newStatus;
            }
        }

        return testStatus switch
        {
            AssessmentStatus.Passed => LearningStatus.Completed,
            AssessmentStatus.Failed => currentStatus == LearningStatus.Completed
                                            ? LearningStatus.Repeat
                                            : LearningStatus.InProgress,
            _ => currentStatus
        };
    }
}
