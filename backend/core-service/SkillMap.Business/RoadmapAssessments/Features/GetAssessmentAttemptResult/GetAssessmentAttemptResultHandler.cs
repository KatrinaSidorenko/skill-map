using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapAssessments.Common;
using SkillMap.Core.RoadmapAssessments;
using SkillMap.Shared.Results;

namespace SkillMap.Business.RoadmapAssessments.Features.GetAssessmentAttemptResult;

[UsedImplicitly]
internal sealed class GetAssessmentAttemptResultHandler(
    IRepository<AssessmentAttempt> attemptRepository,
  IRepository<RoadmapAssessment> assessmentRepository)
    : IRequestHandler<GetAssessmentAttemptResultQuery, AssessmentAttemptResultDto>
{
    public async Task<AssessmentAttemptResultDto> Handle(
        GetAssessmentAttemptResultQuery request,
        CancellationToken cancellationToken)
    {
        var attempt = await attemptRepository.GetByIdAsync(request.AttemptId, cancellationToken)
            ?? throw new ResourceNotFoundException(nameof(AssessmentAttempt), request.AttemptId.ToString());

        if (!attempt.CompletedAt.HasValue)
            throw new InvalidOperationException($"Attempt {request.AttemptId} has not been completed yet.");

        var attemptContent = await attempt.GetAssessmentAttemptResult(cancellationToken);
        var assessment = await assessmentRepository.GetByIdAsync(attempt.AssessmentId, cancellationToken)
            ?? throw new ResourceNotFoundException(nameof(RoadmapAssessment), attempt.AssessmentId.ToString());
        var assessmentContent = await assessment.GetAssessmentContent(cancellationToken);

        var questionLookup = assessmentContent.TopicQuestions?.SelectMany(tq => tq.Questions).ToDictionary(q => q.Id);

        var questionResults = attemptContent.LearningItemsAnalysis?.SelectMany(topic => topic.Value.QuestionsAnalysis.Select(qa => (TopicId: topic.Key, QuestionId: qa.Key, Result: qa.Value)))
            .ToDictionary(
                x => x.QuestionId,
                x =>
                    {
                        var question = questionLookup[x.QuestionId];

                        var answerDetails = question.Answers.ToDictionary(
                        a => a.Id,
                        a => new AnswerDetailDto(
                            AnswerId: a.Id,
                            Text: a.Text,
                            IsCorrect: a.IsCorrect,
                            IsSelected: x.Result.SelectedAnswerId == a.Id));

                        return new QuestionResultDto(
                            QuestionId: x.QuestionId,
                            Text: question.Text,
                            Type: question.Type,
                            IsCorrect: x.Result.IsCorrect,
                            AchievedPoints: x.Result.AchievedPoints,
                            TotalPossiblePoints: x.Result.TotalPossiblePoints,
                            AnswerDetails: answerDetails);
                    });

        return new AssessmentAttemptResultDto(
            AttemptId: attempt.Id,
            WorkspaceId: assessment.RoadmapWorkspaceId.ToString(),
            TotalAchievedPoints: questionResults.Values.Sum(q => q.AchievedPoints),
            TotalPossiblePoints: questionResults.Values.Sum(q => q.TotalPossiblePoints),
            QuestionResults: questionResults);
    }
}