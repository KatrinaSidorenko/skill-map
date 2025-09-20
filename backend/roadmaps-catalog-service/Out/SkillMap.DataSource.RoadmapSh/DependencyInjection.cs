using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillMap.Application.OutPorts.DataSource;

namespace SkillMap.DataSource.RoadmapSh;

public static class DependencyInjection
{
    public static IServiceCollection AddRoadmapShDataSource(this IServiceCollection services)
    {
        services.AddSingleton<IRoadmapDataSource, DataSource>();
        return services;
    }
}
