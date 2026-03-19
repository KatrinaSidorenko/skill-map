using System.Data.SqlTypes;

using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.Application.Abstractions;
using LearningPlatform.RoadmapTests.Service.Application.Models;
using LearningPlatform.RoadmapTests.Service.Infrastructure.Common;
using LearningPlatform.RoadmapTests.Service.Persistence.Abstractions;
using LearningPlatform.RoadmapTests.Service.Persistence.Models;

using SkillMap.Shared.Extensions;

using AnswerDto = LearningPlatform.RoadmapTests.Service.Application.Models.AnswerDto;
using QuestionDto = LearningPlatform.RoadmapTests.Service.Application.Models.QuestionDto;

namespace LearningPlatform.RoadmapTests.Service.Infrastructure.Database;

public class DatabaseQuestionSource(ITopicQuestionsRepository topicQuestionsRepository) : IDatabaseQuestionSource
{
    public async Task<GenerationResult<List<QuestionDto>>> Generate(TopicDto topic, TopicQuestionsSettingDto settings, CancellationToken ct)
    {
        var difficultyLevel = settings.DifficultyLevel.ToDifficultyString();
        var searchId = topic.Id.Trim();
        var searchName = topic.Name.Trim();
        var searchDescription = topic.Description?.Trim();

        try
        {
            var similarTopics = await topicQuestionsRepository.SearchTopicsAsync(
                  searchId: searchId,
                  searchName: searchName,
                  searchDescription: searchDescription,
                  ct);

            if (!similarTopics.Any())
            {
                return new GenerationResult<List<QuestionDto>>(GenerationErrorReasons.NotFound("No similar topics found in the database."));
            }

            var targetQuestionDtos = new List<QuestionDto>(settings.QuestionsCount);
            foreach (var similarTopic in similarTopics)
            {
                if (targetQuestionDtos.Count >= settings.QuestionsCount)
                    break;
                var questions = await topicQuestionsRepository.GetQuestionsByTopicIdAsync(similarTopic.Id, difficultyLevel, ct);

                targetQuestionDtos.AddRange(questions.Select(q => new QuestionDto
                {
                    Id = q.Id.ToString(),
                    Text = q.Text,
                    Type = q.Type.FromQuestionTypeString(),
                    Answers = q.Answers.JsonDeserializeOrDefault<List<AnswerDto>>() ?? new List<AnswerDto>()
                }));
            }

            return new GenerationResult<List<QuestionDto>>(targetQuestionDtos.DistinctBy(t => HashCode.Combine(t.Type, t.Text)).ToList());
        }
        catch (Exception ex)
        {
            return new GenerationResult<List<QuestionDto>>(GenerationErrorReasons.InternalError(ex.Message));
        }
    }
}