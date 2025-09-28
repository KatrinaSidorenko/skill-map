using LearningPlatform.Roadmap.Business.Contracts.Models;
using SkillMap.Shared.Results;

namespace LearningPlatform.Roadmap.Business.Contracts;
public interface IMigrator
{
    Task<bool> MigrateAsync(DataSourceConfig config, CancellationToken ct = default);
    Task<Result<bool>> MigrateNodesDescriptions(DataSourceConfig config, CancellationToken ct);
}
