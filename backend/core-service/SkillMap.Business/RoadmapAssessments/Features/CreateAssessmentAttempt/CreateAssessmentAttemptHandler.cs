using JetBrains.Annotations;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Core.RoadmapAssessments;
using SkillMap.Shared.Results;

namespace SkillMap.Business.RoadmapAssessments.Features.CreateAssessmentAttempt;


[UsedImplicitly]
internal sealed class CreateAssessmentAttemptHandler(
    IRepository<RoadmapAssessment> assessmentRepository,
    IRepository<AssessmentAttempt> attemptRepository)
    : IRequestHandler<CreateAssessmentAttemptCommand, long>
{
    public async Task<long> Handle(CreateAssessmentAttemptCommand request, CancellationToken cancellationToken)
    {
        _ = await assessmentRepository.GetByIdAsync(request.AssessmentId, cancellationToken)
            ?? throw new ResourceNotFoundException(nameof(RoadmapAssessment), request.AssessmentId.ToString());

        //var existingAttempt = await attemptRepository.GetFirstOrDefaultAsync(
        //    a => a.AssessmentId == request.AssessmentId
        //        && a.UserId == request.UserId
        //        && !a.CompletedAt.HasValue,
        //        cancellationToken);

        //if (existingAttempt is not null)
        //    return existingAttempt.Id;

        var attempt = new AssessmentAttempt
        {
            AssessmentId = request.AssessmentId,
            UserId = request.UserId,
            StartedAt = DateTime.UtcNow,
        };

        await attemptRepository.AddAsync(attempt, cancellationToken);
        await attemptRepository.SaveChangesAsync(cancellationToken);

        return attempt.Id;
    }
}