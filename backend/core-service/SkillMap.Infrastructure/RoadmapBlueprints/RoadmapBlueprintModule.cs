using MediatR;

using SkillMap.Business;
using SkillMap.Business.RoadmapBlueprints;

namespace SkillMap.Infrastructure.RoadmapBlueprints;

internal sealed class RoadmapBlueprintModule(ISender mediator) : IRoadmapBlueprintModule
{
    public Task ExecuteCommandAsync(ICommand command, CancellationToken cancellationToken = default)
     => mediator.Send(command, cancellationToken);

    public Task<TResult> ExecuteCommandAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
  => mediator.Send(command, cancellationToken);
}