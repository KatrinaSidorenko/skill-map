using SkillMap.DataSource.RoadmapSh.Configs;

namespace SkillMap.DataSource.RoadmapSh.Api;

public class RoadmapShConstants
{
    public const string BaseUrl = "https://roadmap.sh/";

    private const string MigrationsFileName = "migration-mapping.json";
    public const string MigrationMappingUrl = "https://raw.githubusercontent.com/kamranahmedse/developer-roadmap/refs/heads/master/src/data/roadmaps/";

    public static string GetBaseUrl(string roadmap)
    {
        roadmap = roadmap.ToLowerInvariant().Trim();
        var roadmapJson = roadmap.EndsWith(".json") ? roadmap : $"{roadmap}.json";
        return $"{BaseUrl}{roadmapJson}";
    }

    public static string GetMigrationMappingUrl(string roadmap)
    {
        roadmap = roadmap.ToLowerInvariant().Trim();
        
        return $"{MigrationMappingUrl}{roadmap}/{MigrationsFileName}";
    }

    public static string GetGitHubUrl(GitHubConfig gitHubConfig)
    {
        return $"https://api.github.com/repos/{gitHubConfig.Owner}/{gitHubConfig.Repo}/contents/{gitHubConfig.Path}?ref={gitHubConfig.Branch}";
    }
}
