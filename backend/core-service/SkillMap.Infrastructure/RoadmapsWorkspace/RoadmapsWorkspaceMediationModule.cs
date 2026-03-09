using Microsoft.Extensions.DependencyInjection;

using SkillMap.Business.PersonalizedRoadmaps;
using SkillMap.Business.PersonalRoadmaps;
using SkillMap.Infrastructure.RoadmapsWorkspace;

namespace SkillMap.Infrastructure.PersonalizedRoadmaps;
public static class PersonalRoadmapMediationModule
{
    public static IServiceCollection AddRoadmapsWorkspaceInfrastructure(this IServiceCollection services)
    {
        var commandsHandlersAssembly = typeof(IRoadmapWorkspaceModule).Assembly;
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblies(commandsHandlersAssembly));

        services.AddScoped<IPersonalRoadmapModule, PersonalRoadmapModule>();
        services.AddHostedService<BuildRoadmapWorkspaceSnapshotWorker>();

        return services;
    }
}
