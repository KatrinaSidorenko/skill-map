using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillMap.Application.InPorts.Migrator;
using SkillMap.DataSource.RoadmapSh.Configs;
using SkillMap.Shared;

namespace SkillMap.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MigrationsController : ControllerBase
{
    private IMigrator Migrator { get; }
    public MigrationsController(IMigrator migrator)
    {
        Migrator = migrator ?? throw new ArgumentNullException(nameof(migrator));
    }
    [HttpGet("migrate")]
    // todo: return root node id
    public async Task<IActionResult> Migrate([FromQuery]string roadmapName, CancellationToken ct)
    {
        var config = new DataSourceConfig
        {
            RoadmapName = roadmapName,
        };

        var t = await Migrator.MigrateAsync(config, ct);
        return Ok(t);
    }
    [HttpGet("resources_migration")]
    public async Task<IActionResult> ResourceMigration([FromQuery]string roadmapName, [FromQuery] string roadmapId, CancellationToken ct)
    {
        var config = new DescriptionsDataSourceConfig
        {
            RoadmapId = roadmapId,
            RoadmapName = roadmapName,
            Source = new GitHubConfig
            {
                Owner = "kamranahmedse",
                Repo = "developer-roadmap",
                Path = $"src/data/roadmaps/{roadmapName}/content",
                Branch = "master",
            }
        };


        var t = await Migrator.MigrateNodesDescriptions(config, ct);
        return Ok(t);
    }
}
