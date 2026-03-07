
using MediatR;

using SkillMap.Business;
using SkillMap.Business.PersonalizedRoadmaps;

namespace SkillMap.Infrastructure.PersonalizedRoadmaps;
internal sealed class PersonalizedRoadmapModule(ISender mediator) : IPersonalizedRoadmapModule
{
    public Task ExecuteCommandAsync(ICommand command, CancellationToken cancellationToken = default)
        => mediator.Send(command, cancellationToken);

    public Task<TResult> ExecuteCommandAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
        => mediator.Send(command, cancellationToken);
}
