namespace LearningPlatform.RoadmapTests.Contracts;

public enum TestQuestionType
{
    SingleChoice,
    MultipleChoice,
    TrueFalse,
    FillInTheBlank,
    ShortAnswer,
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}

public static class TestQuestionTypeExtensions
{
    public static string ToQuestionTypeString(this TestQuestionType type)
    {
        return type switch
        {
            TestQuestionType.SingleChoice => "single_choice",
            TestQuestionType.MultipleChoice => "multiple_choice",
            TestQuestionType.TrueFalse => "true_false",
            TestQuestionType.FillInTheBlank => "fill_in_the_blank",
            TestQuestionType.ShortAnswer => "short_answer",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
    public static TestQuestionType FromQuestionTypeString(this string type)
    {
        return type.ToLower() switch
        {
            "single_choice" => TestQuestionType.SingleChoice,
            "multiple_choice" => TestQuestionType.MultipleChoice,
            "true_false" => TestQuestionType.TrueFalse,
            "fill_in_the_blank" => TestQuestionType.FillInTheBlank,
            "short_answer" => TestQuestionType.ShortAnswer,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
