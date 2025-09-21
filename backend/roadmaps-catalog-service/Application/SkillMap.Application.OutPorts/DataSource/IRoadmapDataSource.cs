using SkillMap.Shared.Models;
using SkillMap.Shared;

namespace SkillMap.Application.OutPorts.DataSource;

public interface IRoadmapDataSource
{
    Task<List<FileDataDto>> GetFolderContent(DataSourceConfig config, CancellationToken ct = default);
    Task<Graph> GetRoadmapSource(DataSourceConfig config, CancellationToken cancellationToken = default);
    Task<(string Description, List<ResourceDto> ResourceDtos)> ParseFileContent(FileDataDto fileDataDto, CancellationToken ct);
}
