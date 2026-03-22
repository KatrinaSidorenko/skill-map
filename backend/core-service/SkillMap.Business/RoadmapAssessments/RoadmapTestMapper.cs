using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;

using SkillMap.Business.RoadmapTest.Helpers;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapAssessments;
using SkillMap.Shared.Results;

using SingleAnswerQuestionAnalysisResultDto = SkillMap.Business.RoadmapTest.Models.SingleAnswerQuestionAnalysisResultDto;

namespace SkillMap.Business.RoadmapTest;

public static class RoadmapTestMapper
{
    public static RoadmapTestResultDto ToTestResult(this RoadmapTestDao dao, string testId)
    {
        return new RoadmapTestResultDto
        {
            TestId = testId,
            Questions = dao.TopicQuestions.SelectMany(t => t.Questions.Select(q => new QuestionResultDto
            {
                Id = q.Id,
                TopicId = t.Id,
                Text = q.Text,
                Type = q.Type.ToQuestionTypeString(),
                Answers = q.Answers.Select(a => new AnswerResult
                {
                    Id = a.Id,
                    Text = a.Text
                }).ToList()
            })).ToList()
        };
    }

    public static AnswersCheckResult ToCheckedResults(this RoadmapTestResultsDto analysis)
    {
        return new AnswersCheckResult
        {
            QuestionResults = analysis.TopicsAnalysis.SelectMany(t => t.Value.QuestionsAnalysis).ToDictionary(qa => qa.Key, qa =>
            {
                return qa.Value switch
                {
                    Models.SingleAnswerQuestionAnalysisResultDto single => new CheckedSingleAnswerQuestion
                    {
                        QuestionId = qa.Key,
                        AchievedPoints = single.AchievedPoints,
                        TotalPossiblePoints = single.TotalPossiblePoints,
                        IsCorrect = single.IsCorrect,
                        CorrectAnswerId = single.CorrectAnswerId,
                        QuestionType = single.QuestionType.ToQuestionTypeString()
                    } as CheckedQuestion,
                    _ => throw new LearningPlatformException(ErrorCode.INTERNALERROR, $"Unsupported question type {qa.Value.QuestionType}"),
                };
            })
        };
    }

    public static RoadmapAssessmentContent ToEntityModel(this RoadmapTestDao dao)
    {
        return new RoadmapAssessmentContent
        {
            Config = dao.TestConfig?.ToEntityConfig(),
            TopicSettings = dao.TopicSettings?.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToEntitySetting()),
            TopicQuestions = dao.TopicQuestions?.Select(q => q.ToEntityTopic()).ToList()
        };
    }

    public static RoadmapTestConfig ToEntityConfig(this RoadmapTestConfigDto dto) => new()
    {
        NumberOfQuestions = dto.NumberOfQuestions,
        TimeLimitInMinutes = dto.TimeLimitInMinutes,
        DifficultyLevel = dto.DifficultyLevel
    };

    public static TopicQuestionsSetting ToEntitySetting(this TopicQuestionsSettingDto dto) => new()
    {
        DifficultyLevel = dto.DifficultyLevel.ToDifficultyString(),
        QuestionsCount = dto.QuestionsCount,
        Types = dto.Types.Select(t => t.ToQuestionTypeString()).ToList()
    };

    public static TopicQuestions ToEntityTopic(this TopicQuestionsDto dto) => new()
    {
        TopicId = dto.Id,
        Questions = dto.Questions.Select(q => q.ToEntityQuestion()).ToList()
    };

    public static Question ToEntityQuestion(this QuestionDto dto) => new()
    {
        Id = dto.Id,
        Text = dto.Text,
        Type = dto.Type.ToQuestionTypeString(),
        Answers = dto.Answers.Select(a => a.ToEntityAnswer()).ToList()
    };

    public static Answer ToEntityAnswer(this AnswerDto dto) => new()
    {
        Id = dto.Id,
        Text = dto.Text,
        IsCorrect = dto.IsCorrect
    };

    // --- To DAO Model ---
    public static RoadmapTestDao ToDaoModel(this RoadmapAssessmentContent entity, string testType, string userRoadmapId, long userTestId, string roadmapId)
    {
        return new RoadmapTestDao
        {
            Id = userTestId.ToString(),
            RoadmapId = roadmapId,
            WorkspaceId = userRoadmapId,
            Type = testType,
            TestConfig = entity.Config?.ToDaoConfig(),
            TopicSettings = entity.TopicSettings?.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToDaoSetting()),
            TopicQuestions = entity.TopicQuestions?.Select(q => q.ToDaoTopic()).ToList()
        };
    }

    public static RoadmapTestConfigDto ToDaoConfig(this RoadmapTestConfig entity) => new()
    {
        NumberOfQuestions = entity.NumberOfQuestions,
        TimeLimitInMinutes = entity.TimeLimitInMinutes,
        DifficultyLevel = entity.DifficultyLevel
    };

    public static TopicQuestionsSettingDto ToDaoSetting(this TopicQuestionsSetting entity) => new()
    {
        DifficultyLevel = entity.DifficultyLevel.FromDifficultyString(),
        QuestionsCount = entity.QuestionsCount,
        Types = entity.Types.Select(t => t.FromQuestionTypeString()).ToList()
    };

    public static TopicQuestionsDto ToDaoTopic(this TopicQuestions entity) => new()
    {
        Id = entity.TopicId,
        Questions = entity.Questions.Select(q => q.ToDaoQuestion()).ToList()
    };

    public static QuestionDto ToDaoQuestion(this Question entity) => new()
    {
        Id = entity.Id,
        Text = entity.Text,
        Type = entity.Type.FromQuestionTypeString(),
        Answers = entity.Answers.Select(a => a.ToDaoAnswer()).ToList()
    };

    public static AnswerDto ToDaoAnswer(this Answer entity) => new()
    {
        Id = entity.Id,
        Text = entity.Text,
        IsCorrect = entity.IsCorrect
    };

    // --- Result Conversion ---
    public static AssessmentAttemptContent ToDomainTestResult(this RoadmapTestResultsDto dto)
    {
        return new AssessmentAttemptContent
        {
            TopicsAnalysis = dto.TopicsAnalysis?.ToDictionary(
                kvp => kvp.Key,
                kvp => new TopicAnswersAnalysis
                {
                    QuestionsAnalysis = kvp.Value.QuestionsAnalysis?.ToDictionary(
                        q => q.Key,
                        q => new QuestionAnalysisResult
                        {
                            AchievedPoints = q.Value.AchievedPoints,
                            TotalPossiblePoints = q.Value.TotalPossiblePoints,
                            QuestionType = q.Value.QuestionType.ToQuestionTypeString(),
                            SelectedAnswerId = q.Value.SelectedAnswerId,
                            CorrectAnswerId = q.Value.CorrectAnswerId
                        })
                })
        };
    }

    public static RoadmapTestResultsDto ToDaoResult(this AssessmentAttemptContent entity)
    {
        return new RoadmapTestResultsDto(entity.TopicsAnalysis?.ToDictionary(
                kvp => kvp.Key,
                kvp => new TopicAnswersAnalysisDto(kvp.Value.QuestionsAnalysis?.ToDictionary(
                        q => q.Key,
                        q => new Models.QuestionAnalysisResultDto
                        {
                            AchievedPoints = q.Value.AchievedPoints,
                            TotalPossiblePoints = q.Value.TotalPossiblePoints,
                            QuestionType = q.Value.QuestionType.FromQuestionTypeString(),
                            SelectedAnswerId = q.Value.SelectedAnswerId,
                            CorrectAnswerId = q.Value.CorrectAnswerId
                        }))));
    }

    public static TestEstimationResult ToTestEstimationResult(this RoadmapTestResultsDto result, RoadmapTestDao roadmapTest)
    {
        var analysisByQuestion = result.TopicsAnalysis
            .SelectMany(t => t.Value.QuestionsAnalysis)
            .ToDictionary(qa => qa.Key, qa => qa.Value);
        return new TestEstimationResult
        {
            TopicsAnalysis = result.TopicsAnalysis,
            QuestionResults = roadmapTest.TopicQuestions
                .SelectMany(t => t.Questions)
                .ToDictionary(q => q.Id, q => BuildQuestionResult(q, analysisByQuestion[q.Id])),
            WorkspaceId = roadmapTest.WorkspaceId,
        };
    }

    public static string ToLearningStatusString(this NodeMarkType markType)
    {
        return markType switch
        {
            NodeMarkType.Finished => LearningStatus.Completed.ToStatusString(),
            NodeMarkType.NeedsReview => LearningStatus.NotStarted.ToStatusString(),
            NodeMarkType.Uncertain => LearningStatus.InProgress.ToStatusString(),
            _ => null
        };
    }
    private static TestQuestionResult BuildQuestionResult(QuestionDto question, QuestionAnalysisResultDto analysis)
    {
        return new TestQuestionResult
        {
            Type = analysis.QuestionType.ToQuestionTypeString(),
            TotalPossiblePoints = analysis.TotalPossiblePoints,
            AchievedPoints = analysis.AchievedPoints,
            IsCorrect = analysis.AchievedPoints >= analysis.TotalPossiblePoints,
            QuestionId = question.Id,
            Text = question.Text,
            AnswerDetails = question.Answers
                .Select(answer => BuildAnswerDetail(question, answer, analysis))
                .ToDictionary(ad => ad.AnswerId, ad => ad)
        };
    }

    private static AnswerDetail BuildAnswerDetail(QuestionDto question, AnswerDto answer, QuestionAnalysisResultDto analysis)
    {
        switch (question.Type)
        {
            case TestQuestionType.SingleChoice:
                {
                    var singleChoiceAnalysis = analysis as SingleAnswerQuestionAnalysisResultDto;
                    return new SingleChoiceAnswerDetail
                    {
                        Text = answer.Text,
                        AnswerId = answer.Id,
                        IsCorrect = answer.IsCorrect,
                        IsSelected = analysis.SelectedAnswerId == answer.Id
                    };
                }
            default:
                throw new LearningPlatformException(ErrorCode.INTERNALERROR, $"Unsupported question type {question.Type}");
        }
    }
}