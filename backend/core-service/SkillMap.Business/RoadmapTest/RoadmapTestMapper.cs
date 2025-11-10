using LearningPlatform.RoadmapTests.Contracts;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Shared.Results;

namespace SkillMap.Business.RoadmapTest;

public static class RoadmapTestMapper
{
    public static RoadmapTestResult ToTestResult(this RoadmapTestDao dao)
    {
        return new RoadmapTestResult
        {
            TestId = dao.Id,
            Questions = dao.TopicQuestions.SelectMany(t => t.Questions.Select(q => new QuestionResult
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

    public static AnswersCheckResult ToCheckedResults(this TestAnswersAnalysis analysis)
    {
        return new AnswersCheckResult
        {
            QuestionResults = analysis.TopicsAnalysis.SelectMany(t => t.Value.QuestionsAnalysis).ToDictionary(qa => qa.Key, qa =>
            {
                return qa.Value switch
                {
                    SingleAnswerQuestionAnalysisResult single => new CheckedSingleAnswerQuestion
                    {
                        QuestionId = qa.Key,
                        AchievedPoints = single.AchievedPoints,
                        TotalPossiblePoints = single.TotalPossiblePoints,
                        IsCorrect = single.IsCorrect,
                        CorrectAnswerId = single.CorrectAnswerId,
                    } as CheckedQuestion,
                    _ => throw new LearningPlatformException(ErrorCode.INTERNAL_ERROR, $"Unsupported question type {qa.Value.QuestionType}"),
                };
            })
        };
    }
}
