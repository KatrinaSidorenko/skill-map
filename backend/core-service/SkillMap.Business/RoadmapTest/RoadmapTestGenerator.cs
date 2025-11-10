using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;
using Newtonsoft.Json;

namespace SkillMap.Business.RoadmapTest;

public class RoadmapTestGenerator : IRoadmapTestGenerator
{
    public async Task<List<TopicQuestionsDto>> GenerateRoadmapTest(
            List<(Topic topic, TopicQuestionsSettingDto settings)> topicsSettings,
            CancellationToken ct)
    {
        // Simulate async I/O operation (e.g., external question source)
        await Task.Delay(100, ct);

        // Generate mock topics with questions
        var mockData = topicsSettings.Select(t =>
        {
            var topic = t.topic;
            var settings = t.settings;
            var topicQuestions = GenerateTopicQuestions(topic, settings);
            return topicQuestions;
        }).ToList();

        // Serialize to JSON and back (to simulate data flow or cache usage)
        var json = JsonConvert.SerializeObject(mockData, Formatting.Indented);
        var deserialized = JsonConvert.DeserializeObject<List<TopicQuestionsDto>>(json);

        return deserialized ?? new List<TopicQuestionsDto>();
    }

    private TopicQuestionsDto GenerateTopicQuestions(Topic topic, TopicQuestionsSettingDto settings)
    {
        var random = new Random();
        var questions = new List<QuestionDto>();

        for (int i = 1; i <= settings.QuestionsCount; i++)
        {
            var questionId = $"{topic.Id}-q{i}";
            var questionText = $"[{settings.DifficultyLevel.ToDifficultyString()}] Question {i} about {topic.Name}?";

            var answers = Enumerable.Range(1, 3)
                .Select(a => new AnswerDto
                {
                    Id = $"{questionId}-a{a}",
                    Text = $"Option {a} for {topic.Name}",
                    IsCorrect = a == 1 // always first one correct for mock
                })
                .ToList();

            questions.Add(new QuestionDto
            {
                Id = questionId,
                Text = questionText,
                Answers = answers
            });
        }

        return new TopicQuestionsDto
        {
            Id = topic.Id,
            Questions = questions
        };
    }

    public Task<TopicQuestionsDto> GenerateTopicQuestions(Topic topic, TopicQuestionsSettingDto settings, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
