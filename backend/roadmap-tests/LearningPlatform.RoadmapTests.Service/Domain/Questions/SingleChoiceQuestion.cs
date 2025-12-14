using LearningPlatform.RoadmapTests.Contracts;
using SkillMap.Shared.Results;

namespace LearningPlatform.RoadmapTests.Service.Core.Questions;

public sealed class SingleChoiceQuestion : Question
{
    public IReadOnlyList<ChoiceAnswer> Answers { get; }

    private SingleChoiceQuestion(
        string id,
        string text,
        Difficulty difficulty,
        IReadOnlyList<ChoiceAnswer> answers)
        : base(id, text, difficulty, TestQuestionType.SingleChoice)
    {
        Answers = answers;
    }

    public static SingleChoiceQuestion Create(
        string id,
        string text,
        Difficulty difficulty,
        IEnumerable<ChoiceAnswer> answers)
    {
        var list = answers.ToList();

        if (list.Count < 2)
            throw new LearningPlatformException(ErrorCode.VALIDATION_ERROR, "At least 2 answers required");

        if (list.Count(a => a.IsCorrect) != 1)
            throw new LearningPlatformException(ErrorCode.VALIDATION_ERROR, "Exactly one correct answer required");

        return new SingleChoiceQuestion(id, text, difficulty, list);
    }

    //public override bool Evaluate(AnswerSubmission submission)
    //{
    //    return submission.SelectedAnswerIds.Count == 1 &&
    //           Answers.Any(a =>
    //               a.IsCorrect &&
    //               submission.SelectedAnswerIds.Contains(a.Id));
    //}
}
