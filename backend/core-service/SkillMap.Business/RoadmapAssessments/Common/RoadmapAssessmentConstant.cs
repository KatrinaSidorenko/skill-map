using LearningPlatform.RoadmapTests.Contracts;

namespace SkillMap.Business.RoadmapAssessments.Common;

public class RoadmapAssessmentConstant
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