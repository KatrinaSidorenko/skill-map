using Microsoft.Extensions.DependencyInjection;

using SkillMap.Business.PersonalizedRoadmaps;
using SkillMap.Business.PersonalRoadmaps;
using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Infrastructure.RoadmapsWorkspace;

namespace SkillMap.Infrastructure.PersonalizedRoadmaps;
public static class PersonalRoadmapMediationModule
{
    public static IServiceCollection AddRoadmapsWorkspaceInfrastructure(this IServiceCollection services)
    {
        var commandsHandlersAssembly = typeof(Business.PersonalizedRoadmaps.IRoadmapWorkspaceModule).Assembly;
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblies(commandsHandlersAssembly));

        services.AddScoped<Business.PersonalRoadmaps.IRoadmapWorkspaceModule, PersonalRoadmapModule>();
        services.AddHostedService<BuildRoadmapWorkspaceSnapshotWorker>();
        services.AddHostedService<ProcessWorkspaceEventsWorker>();

        services.AddScoped<IRoadmapWorkspaceEventRepository, RoadmapWorkspaceEventRepository>();
        services.AddScoped<IRoadmapWorkspaceRepository, RoadmapWorkspaceRepository>();
        services.AddScoped<IRoadmapWorkspaceSnapshotRepository, RoadmapWorkspaceSnapshotRepository>();

        services.AddScoped<IRoadmapWorkspaceEditor, RoadmapWorkspaceEditor>();

        return services;
    }
}
