namespace SkillMap.Business.RoadmapAssessments.Common.SelectLearningItems;

/// <summary>
/// Randomly samples subtopics from the Completed and Assumed pools.
/// </summary>
internal static class SimpleSubtopicSelector
{
    internal static List<LeaningItemAssessment> Select(
        SubtopicPools pools,
        QuotaAllocation quota,
        Random rnd)
    {
        var selected = new List<LeaningItemAssessment>(quota.TakeCompleted + quota.TakeAssumed);

        selected.AddRange(pools.Completed.OrderBy(_ => rnd.Next()).Take(quota.TakeCompleted));
        selected.AddRange(pools.Assumed.OrderBy(_ => rnd.Next()).Take(quota.TakeAssumed));

        return selected;
    }
}