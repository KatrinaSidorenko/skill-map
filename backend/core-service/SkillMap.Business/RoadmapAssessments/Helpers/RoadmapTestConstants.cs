using LearningPlatform.RoadmapTests.Contracts;

namespace SkillMap.Business.RoadmapTest.Helpers;

public class RoadmapTestConstants
{
    public const int MinAmountOfTopics = 3;
    public const int MaxQuestionsPerTopic = 3;
    public const int MaxNumberOfQuestions = 20;
    public const double MinMinutesPerQuestion = 0.5;
    public static HashSet<TestQuestionType> SupportedQuestionTypes = new()
    {
        TestQuestionType.SingleChoice,
    };
}