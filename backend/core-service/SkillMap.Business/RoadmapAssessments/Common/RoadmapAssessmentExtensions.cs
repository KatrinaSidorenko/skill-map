using SkillMap.Core.RoadmapAssessments;
using SkillMap.Shared.Gzip;

using RoadmapTestDao = SkillMap.Core.RoadmapAssessments.RoadmapAssessmentContent;
using UserRoadmpTestDao = SkillMap.Core.RoadmapAssessments.RoadmapAssessment;

namespace SkillMap.Business.RoadmapAssessments.Common;

public static class RoadmapAssessmentExtensions
{
    public static async Task<RoadmapTestDao> GetAssessmentContent(this UserRoadmpTestDao userRoadmapTest, CancellationToken ct)
    {
        if (userRoadmapTest.TestData == null || userRoadmapTest.TestData.Length == 0)
        {
            throw new InvalidOperationException("TestData is null or empty.");
        }

        return await userRoadmapTest.TestData.InGzipJsonObjectUtf8<RoadmapTestDao>(ct);
    }

    public static async Task SetRoadmapTest(this UserRoadmpTestDao userRoadmapTest, RoadmapTestDao roadmapTest, CancellationToken ct)
    {
        if (roadmapTest == null)
        {
            throw new ArgumentNullException(nameof(roadmapTest), "RoadmapTest cannot be null.");
        }
        userRoadmapTest.TestData = await roadmapTest.GzipJsonObjectUtf8(ct);
    }

    public static async Task<AssessmentAttemptContent> GetAssessmentAttemptResult(this AssessmentAttempt userTestResult, CancellationToken ct)
    {
        if (userTestResult.ResultData == null || userTestResult.ResultData.Length == 0)
        {
            throw new InvalidOperationException("ResultData is null or empty.");
        }
        return await userTestResult.ResultData.InGzipJsonObjectUtf8<AssessmentAttemptContent>(ct);
    }

    public static async Task SetAssessmentAttemptResult(this AssessmentAttempt userTestResult, AssessmentAttemptContent testResult, CancellationToken ct)
    {
        if (testResult == null)
        {
            throw new ArgumentNullException(nameof(testResult), "TestResult cannot be null.");
        }

        userTestResult.MaxPoints = testResult.TotalPossiblePoints;
        userTestResult.ScoredPoints = testResult.AchievedPoints;
        userTestResult.ResultData = await testResult.GzipJsonObjectUtf8(ct);
    }
}