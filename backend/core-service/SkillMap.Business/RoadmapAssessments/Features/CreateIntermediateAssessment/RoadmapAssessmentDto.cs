using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;

using SkillMap.Core.RoadmapAssessments;

namespace SkillMap.Business.RoadmapAssessments.Features.CreateIntermediateAssessment;

internal class RoadmapAssessmentDto
{
    public string RoadmapId { get; set; }
    public string WorkspaceId { get; set; }
    public string Type { get; set; }
    public List<TopicQuestionsDto> TopicQuestions { get; set; }
    public Dictionary<string, TopicQuestionsSettingDto> TopicSettings { get; set; }
    public RoadmapAssessmentConfigDto TestConfig { get; set; }

    public RoadmapAssessmentContent ToEntityModel()
    {
        return new RoadmapAssessmentContent
        {
            Config = TestConfig?.ToEntityConfig(),
            TopicSettings = TopicSettings?.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToEntitySetting()),
            TopicQuestions = TopicQuestions?.Select(tq => tq.ToEntityTopicQuestions()).ToList()
        };
    }
}

internal static class RoadmapAssessmentDtoExtensions
{
    internal static RoadmapTestConfig ToEntityConfig(this RoadmapAssessmentConfigDto dto) => new()
    {
        NumberOfQuestions = dto.NumberOfQuestions,
        TimeLimitInMinutes = dto.TimeLimitInMinutes,
        DifficultyLevel = dto.DifficultyLevel
    };

    internal static TopicQuestionsSetting ToEntitySetting(this TopicQuestionsSettingDto dto) => new()
    {
        DifficultyLevel = dto.DifficultyLevel.ToDifficultyString(),
        QuestionsCount = dto.QuestionsCount,
        Types = dto.Types.Select(t => t.ToQuestionTypeString()).ToList()
    };

    internal static TopicQuestions ToEntityTopicQuestions(this TopicQuestionsDto dto) => new()
    {
        TopicId = dto.Id,
        Questions = dto.Questions.Select(q => q.ToEntityQuestion()).ToList()
    };

    internal static Question ToEntityQuestion(this QuestionDto dto) => new()
    {
        Id = dto.Id,
        Text = dto.Text,
        Type = dto.Type.ToQuestionTypeString(),
        Answers = dto.Answers.Select(a => a.ToEntityAnswer()).ToList()
    };

    internal static Answer ToEntityAnswer(this AnswerDto dto) => new()
    {
        Id = dto.Id,
        Text = dto.Text,
        IsCorrect = dto.IsCorrect
    };
}