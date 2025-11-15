using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillMap.Business.Abstractions;
using SkillMap.Persistence.Neo4j;

namespace SkillMap.Persistence;

public static class LayerRegistration
{
    public static IServiceCollection AddPersistenceLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SkillMapDbContext>(options =>
                   options
                   .UseLazyLoadingProxies()
                   .UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddNeo4jPersistence(configuration);
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
