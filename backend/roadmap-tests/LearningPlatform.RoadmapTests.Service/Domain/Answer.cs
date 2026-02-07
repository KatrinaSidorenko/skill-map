namespace LearningPlatform.RoadmapTests.Service.Core;

public abstract class Answer
{
    public string Id { get; }
    public string Text { get; }

    protected Answer(string id, string text)
    {
        Id = id;
        Text = text;
    }
}

public sealed class ChoiceAnswer : Answer
{
    public bool IsCorrect { get; }

    public ChoiceAnswer(string id, string text, bool isCorrect)
        : base(id, text)
    {
        IsCorrect = isCorrect;
    }
}