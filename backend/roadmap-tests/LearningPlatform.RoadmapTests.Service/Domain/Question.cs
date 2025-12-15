using LearningPlatform.RoadmapTests.Contracts;
using SkillMap.Shared.Results;

namespace LearningPlatform.RoadmapTests.Service.Core;

public abstract class Question
{
    public long Id { get; }
    public string Text { get; }
    public Difficulty Difficulty { get; }
    public TestQuestionType Type { get; }
    // how to store answers content in jsonb

    protected Question(
        string text,
        Difficulty difficulty,
        TestQuestionType type)
    {
        //if (string.IsNullOrWhiteSpace(id))
        //    throw new LearningPlatformException(ErrorCode.VALIDATION_ERROR, "Question id is required");

        if (string.IsNullOrWhiteSpace(text))
            throw new LearningPlatformException(ErrorCode.VALIDATION_ERROR, "Question text is required");

        Text = text;
        Difficulty = difficulty;
        Type = type;
    }

}
