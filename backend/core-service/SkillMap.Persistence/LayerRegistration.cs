using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SkillMap.Persistence;

public static class LayerRegistration
{
    public static IServiceCollection AddPersistenceLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SkillMapDbContext>(options =>
                   options
                   .UseLazyLoadingProxies()
                   .UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
       

        return services;
    }
}