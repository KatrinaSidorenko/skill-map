using SkillMap.Shared;

namespace SkillMap.DataSource.RoadmapSh.Configs;

public class DescriptionsDataSourceConfig : DataSourceConfig
{
    public string RoadmapRootNodeId { get; set; }
    public GitHubConfig Source { get; set; }
}
