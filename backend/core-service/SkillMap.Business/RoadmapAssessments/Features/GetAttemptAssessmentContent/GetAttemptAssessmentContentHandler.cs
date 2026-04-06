using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapAssessments.Common;
using SkillMap.Business.RoadmapAssessments.Features.GetRoadmapAssessment;
using SkillMap.Core.RoadmapAssessments;

namespace SkillMap.Business.RoadmapAssessments.Features.GetAssessment;


[UsedImplicitly]
internal sealed class GetAttemptAssessmentContentHandler(
    IRepository<RoadmapAssessment> repository, IRepository<AssessmentAttempt> assessmentAttemptRepository) : IRequestHandler<GetAttemptAssessmentContentQuery, RoadmapAssessmentDto>
{
    public async Task<RoadmapAssessmentDto> Handle(GetAttemptAssessmentContentQuery request, CancellationToken cancellationToken)
    {
        var attempt = await assessmentAttemptRepository.GetByIdAsync(request.AttemptId, cancellationToken)
            ?? throw new ArgumentException($"Assessment attempt with id {request.AttemptId} not found.");
        var assessment = await repository.GetByIdAsync(attempt.AssessmentId, cancellationToken)
            ?? throw new ArgumentException($"Roadmap assessment with id {attempt.AssessmentId} not found.");

        var assessmentContent = await assessment.GetAssessmentContent(cancellationToken);
        return RoadmapAssessmentDto.Create(assessment.Id, assessmentContent.TopicQuestions.SelectMany(tq => tq.Questions).ToList());
    }
}