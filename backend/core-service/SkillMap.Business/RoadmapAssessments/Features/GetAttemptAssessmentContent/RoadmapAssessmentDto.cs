using LearningPlatform.RoadmapTests.Contracts;

using SkillMap.Core.RoadmapAssessments;

namespace SkillMap.Business.RoadmapAssessments.Features.GetRoadmapAssessment;
public record RoadmapAssessmentDto(
    long AssessmentId,
    List<RoadmapAssessmentQuestionDto> Questions)
{
    public static RoadmapAssessmentDto Create(long assessmentId, List<Question> questions)
    {
        return new RoadmapAssessmentDto(
            assessmentId,
            questions.Select(q => new RoadmapAssessmentQuestionDto(
                q.Id,
                q.Text,
                q.Type,
                q.Answers.Select(a => new RoadmapAssessmentAnswerDto(a.Id, a.Text)).ToList()
            )).ToList()
        );
    }
}

public record RoadmapAssessmentQuestionDto(
    string Id,
    string Text,
    string Type,
    List<RoadmapAssessmentAnswerDto> Answers
);

public record RoadmapAssessmentAnswerDto(
    string Id,
    string Text
);