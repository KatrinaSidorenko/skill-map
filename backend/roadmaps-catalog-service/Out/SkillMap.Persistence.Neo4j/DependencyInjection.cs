using LearningPlatform.Roadmap.Business.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver;

namespace SkillMap.Persistence.Neo4j;

public static class DependencyInjection
{
    public static IServiceCollection AddNeo4jPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("Neo4j");
        var db = section["Database"];
        var username = section["Username"];
        var password = section["Password"];
        var server = section["Server"];

        services.AddSingleton<IDriver>(s =>
        {
            var uri = new Uri(server);
            return GraphDatabase.Driver(uri, AuthTokens.Basic(username, password));
        });

        services.AddTransient<IRoadmapRepository, RoadmapRepository>(p =>
        {
            var driver = p.GetRequiredService<IDriver>();
            var logger = p.GetRequiredService<Serilog.ILogger>();
            return new RoadmapRepository(driver, new DbSettings(db), logger);
        });

        return services;
    }
}
