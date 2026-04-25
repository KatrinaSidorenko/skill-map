namespace SkillMap.Business.RoadmapsWorkspace;
public interface IRoadmapWorkspaceActionConsumer
{
    Task ConsumeAsync(CancellationToken ct);
}
