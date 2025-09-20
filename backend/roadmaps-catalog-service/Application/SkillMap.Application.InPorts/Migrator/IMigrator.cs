using SkillMap.Shared;
using SkillMap.Shared.Results;

namespace SkillMap.Application.InPorts.Migrator;
public interface IMigrator
{
    Task<bool> MigrateAsync(DataSourceConfig config, CancellationToken ct = default);
    Task<Result<bool>> MigrateNodesDescriptions(DataSourceConfig config, CancellationToken ct);
}
