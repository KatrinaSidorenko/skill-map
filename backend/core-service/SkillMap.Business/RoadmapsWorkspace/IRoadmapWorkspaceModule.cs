namespace SkillMap.Business.PersonalizedRoadmaps;
public interface IRoadmapWorkspaceModule
{
    Task ExecuteCommandAsync(ICommand command, CancellationToken cancellationToken = default);

    Task<TResult> ExecuteCommandAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
}
