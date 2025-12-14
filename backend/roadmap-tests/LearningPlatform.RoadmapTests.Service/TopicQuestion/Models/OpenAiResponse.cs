namespace LearningPlatform.RoadmapTests.Service.TopicQuestion.Models
{
    public sealed class OpenAiQuestionsResponse
    {
        public List<OpenAiQuestion> Questions { get; set; }
    }

    public sealed class OpenAiQuestion
    {
        public string Text { get; set; }
        public string Type { get; set; }
        public List<OpenAiAnswer> Answers { get; set; }
    }

    public sealed class OpenAiAnswer
    {
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }

}
