using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JetBrains.Annotations;

using LearningPlatform.RoadmapTests.Contracts;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapAssessments.Common;
using SkillMap.Core.RoadmapAssessments;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Results;

using CoreQuestionAnalysisResult = SkillMap.Core.RoadmapAssessments.QuestionAnalysisResult;
using CoreTopicAnswersAnalysis = SkillMap.Core.RoadmapAssessments.TopicAnswersAnalysis;

namespace SkillMap.Business.RoadmapAssessments.Features.EvaluateRoadmapAssessment;

[UsedImplicitly]
internal sealed class EvaluateRoadmapAssessmentHandler(
    IRepository<RoadmapAssessment> assessmentRepository,
    IRepository<AssessmentAttempt> attemptRepository)
    : IRequestHandler<EvaluateRoadmapAssessmentCommand, long>
{
    public async Task<long> Handle(EvaluateRoadmapAssessmentCommand request, CancellationToken cancellationToken)
    {
        var attempt = await attemptRepository.GetByIdAsync(request.AttemptId, cancellationToken)
            ?? throw new ResourceNotFoundException(nameof(AssessmentAttempt), request.AttemptId.ToString());
        var assessment = await assessmentRepository.GetByIdAsync(attempt.AssessmentId, cancellationToken)
            ?? throw new ResourceNotFoundException(nameof(RoadmapAssessment), attempt.AssessmentId.ToString());
        var content = await assessment.GetAssessmentContent(cancellationToken);
        
        var answersByQuestionId = request.ProvidedAnswers.ToDictionary(a => a.QuestionId);
        var questionResults = content.TopicQuestions
            .SelectMany(tq => tq.Questions)
            .ToDictionary(
            q => q.Id,
            q => ScoreQuestion(q, answersByQuestionId.GetOrDefault(q.Id)));
        var analysisResult = new AssessmentAttemptContent
        {
            TopicsAnalysis = content.TopicQuestions.ToDictionary(
             tq => tq.TopicId,
            tq => new CoreTopicAnswersAnalysis
            {
                QuestionsAnalysis = tq.Questions.ToDictionary(
              q => q.Id,
                 q => questionResults[q.Id])
            })
        };

        // 6. Stamp and persist the attempt
        attempt.CompletedAt = DateTime.UtcNow;
        await attempt.SetAssessmentAttemptResult(analysisResult, cancellationToken);

        await attemptRepository.UpdateAsync(attempt, cancellationToken);
        await attemptRepository.SaveChangesAsync(cancellationToken);

        return attempt.Id;
    }

    // -------------------------------------------------------------------------
    // Scoring
    // -------------------------------------------------------------------------

    private static CoreQuestionAnalysisResult ScoreQuestion(Question question, ProvidedAnswer? answer)
    {
        return question.Type.FromQuestionTypeString() switch
        {
            TestQuestionType.SingleChoice => ScoreSingleChoice(question, answer),
            _ => throw new InvalidOperationException($"Unsupported question type: {question.Type}")
        };
    }

    private static CoreQuestionAnalysisResult ScoreSingleChoice(Question question, ProvidedAnswer? answer)
    {
        var correctAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect);
        var isCorrect = correctAnswer is not null
                && answer?.SelectedAnswerId is not null
         && answer.SelectedAnswerId == correctAnswer.Id;

        return new CoreQuestionAnalysisResult
        {
            QuestionType = TestQuestionType.SingleChoice.ToQuestionTypeString(),
            TotalPossiblePoints = 1,
            AchievedPoints = isCorrect ? 1 : 0,
            SelectedAnswerId = answer?.SelectedAnswerId,
            CorrectAnswerId = correctAnswer?.Id,
        };
    }
}