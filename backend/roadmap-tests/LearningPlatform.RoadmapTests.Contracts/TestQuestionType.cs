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
    public const string QuestionTypeSingleChoice = "single_choice";
    public const string QuestionTypeMultipleChoice = "multiple_choice";
    public const string QuestionTypeTrueFalse = "true_false";
    public const string QuestionTypeFillInTheBlank = "fill_in_the_blank";
    public const string QuestionTypeShortAnswer = "short_answer";

    private static readonly HashSet<TestQuestionType> AvailableQuestionTypes = new()
    {
        TestQuestionType.SingleChoice,
        TestQuestionType.MultipleChoice,
        TestQuestionType.TrueFalse,
        TestQuestionType.FillInTheBlank,
        TestQuestionType.ShortAnswer
    };
    public static bool IsAvailableQuestionType(this TestQuestionType type)
    {
        return type switch
        {
            TestQuestionType.SingleChoice => true,
            TestQuestionType.MultipleChoice => true,
            TestQuestionType.TrueFalse => true,
            TestQuestionType.FillInTheBlank => true,
            TestQuestionType.ShortAnswer => true,
            _ => false
        };
    }

    public static bool IsAvailableQuestionType(this string type)
    {
        return type.ToLower() switch
        {
            QuestionTypeSingleChoice => true,
            QuestionTypeMultipleChoice => true,
            QuestionTypeTrueFalse => true,
            QuestionTypeFillInTheBlank => true,
            QuestionTypeShortAnswer => true,
            _ => false
        };
    }

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

    public static string ToDifficultyString(this Difficulty difficulty)
    {
        return difficulty switch
        {
            Difficulty.Easy => Difficulty.Easy.ToString().ToLower(),
            Difficulty.Medium => Difficulty.Medium.ToString().ToLower(),
            Difficulty.Hard => Difficulty.Hard.ToString().ToLower(),    
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null)
        };
    }

    public static Difficulty FromDifficultyString(this string difficulty)
    {
        return difficulty.ToLower() switch
        {
            "easy" => Difficulty.Easy,
            "medium" => Difficulty.Medium,
            "hard" => Difficulty.Hard,
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null)
        };
    }
}
