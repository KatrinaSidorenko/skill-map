using SkillMap.Core.Entities.UserRoadmapTest;
using SkillMap.Shared.Gzip;
using RoadmapTestDao = SkillMap.Core.Entities.UserRoadmapTest.RoadmapTest;
using UserRoadmpTestDao = SkillMap.Core.Entities.UserRoadmapTest.UserRoadmapTest;

namespace SkillMap.Business.RoadmapTest;

public static class RoadmapTestExtensions
{
    public static async Task<RoadmapTestDao> GetRoadmapTest(this UserRoadmpTestDao userRoadmapTest, CancellationToken ct)
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

    public static async Task<RoadmapTestResult> GetTestResults(this UserTestResult userTestResult, CancellationToken ct)
    {
        if (userTestResult.ResultData == null || userTestResult.ResultData.Length == 0)
        {
            throw new InvalidOperationException("ResultData is null or empty.");
        }
        return await userTestResult.ResultData.InGzipJsonObjectUtf8<RoadmapTestResult>(ct);
    }

    public static async Task SetTestResults(this UserTestResult userTestResult, RoadmapTestResult testResult, CancellationToken ct)
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
