using MediatR;

using SkillMap.Business;
using SkillMap.Business.RoadmapAssessments;

namespace SkillMap.Infrastructure.RoadmapAssessments;

internal sealed class RoadmapAssessmentModule(ISender mediator) : IRoadmapAssessmentModule
{
    public Task ExecuteCommandAsync(ICommand command, CancellationToken cancellationToken = default)
          => mediator.Send(command, cancellationToken);

    public Task<TResult> ExecuteCommandAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
        => mediator.Send(command, cancellationToken);
}