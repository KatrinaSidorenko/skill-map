using SkillMap.Core.Constants;

namespace SkillMap.Business.RoadmapAssessments.Common.SelectLearningItems;

internal sealed record SubtopicPools(
List<LeaningItemAssessment> Completed,
    List<LeaningItemAssessment> Assumed,
    List<LeaningItemAssessment> Frontier);

internal sealed record QuotaAllocation(
    int TakeCompleted,
    int TakeAssumed,
    int TakeFrontier);

/// <summary>
/// Stratifies assessed subtopics into three pools (Completed, Assumed, Frontier)
/// and calculates how many to draw from each pool with forward and backward cascading.
/// </summary>
internal static class QuestionDistributionCalculator
{
    private const double CompletedRatio = 0.20;
    private const double AssumedRatio = 0.30;

    internal static SubtopicPools BuildPools(List<LeaningItemAssessment> subtopics)
    {
        subtopics = subtopics.Where(st => st.Status != LearningStatus.Skip).ToList();

        var completed = subtopics
           .Where(s => s.Status == LearningStatus.Completed && s.Assumption == null)
            .ToList();

        var assumed = subtopics
            .Where(s => s.Assumption == AssessmentAssumption.AssumedCompleted)
            .ToList();

        var frontier = subtopics
            .Where(s => s.Status != LearningStatus.Completed && s.Assumption != AssessmentAssumption.AssumedCompleted)
            .ToList();

        return new SubtopicPools(completed, assumed, frontier);
    }

    internal static QuotaAllocation CalculateQuotas(SubtopicPools pools, int totalQuestions)
    {
        int targetCompleted = (int)Math.Round(totalQuestions * CompletedRatio);
        int targetAssumed = (int)Math.Round(totalQuestions * AssumedRatio);
        int targetFrontier = totalQuestions - targetCompleted - targetAssumed;

        // Forward cascade: unfilled slots spill into the next tier
        int takeCompleted = Math.Min(targetCompleted, pools.Completed.Count);
        int remainder = targetCompleted - takeCompleted;

        int takeAssumed = Math.Min(targetAssumed + remainder, pools.Assumed.Count);
        remainder = targetAssumed + remainder - takeAssumed;

        int takeFrontier = Math.Min(targetFrontier + remainder, pools.Frontier.Count);
        remainder = targetFrontier + remainder - takeFrontier;

        // Backward cascade: if Frontier is still exhausted, backfill Assumed then Completed
        if (remainder > 0)
        {
            int extraAssumed = Math.Min(remainder, pools.Assumed.Count - takeAssumed);
            takeAssumed += extraAssumed;
            remainder -= extraAssumed;

            int extraCompleted = Math.Min(remainder, pools.Completed.Count - takeCompleted);
            takeCompleted += extraCompleted;
        }

        return new QuotaAllocation(takeCompleted, takeAssumed, takeFrontier);
    }
}