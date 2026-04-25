using System.Text.Json;

using LearningPlatform.Workspace.WebSockets;
using LearningPlatform.Workspace.WebSockets.Contracts;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Infrastructure.RoadmapsWorkspace;

namespace SkillMap.Infrastructure.PersonalizedRoadmaps;
public static class PersonalRoadmapMediationModule
{
    public static IServiceCollection AddRoadmapsWorkspaceInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var commandsHandlersAssembly = typeof(Business.PersonalizedRoadmaps.IRoadmapWorkspaceModule).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(commandsHandlersAssembly));

        services.AddScoped<Business.PersonalRoadmaps.IRoadmapWorkspaceModule, PersonalRoadmapModule>();
        services.AddHostedService<BuildRoadmapWorkspaceSnapshotWorker>();

        services.AddScoped<IRoadmapWorkspaceEventRepository, RoadmapWorkspaceEventRepository>();
        services.AddScoped<IRoadmapWorkspaceRepository, RoadmapWorkspaceRepository>();
        services.AddScoped<IRoadmapWorkspaceSnapshotRepository, RoadmapWorkspaceSnapshotRepository>();
        services.AddScoped<IRoadmapLearningItemProjectionRepository, RoadmapLearningItemProjectionRepository>();
        services.AddScoped<IRoadmapWorkspaceEditor, RoadmapWorkspaceEditor>();

        services.AddSingleton<IWorkspaceNotifier, WorkspaceNotifier>();
        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
        })
        .AddJsonProtocol(options =>
        {
            options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        services.Configure<RoadmapWorkspaceActionConsumerOptions>(configuration.GetSection(RoadmapWorkspaceActionConsumerOptions.SectionName));
        services.AddHostedService<RoadmapWorkspaceActionConsumer>();

        services.Configure<RoadmapWorkspaceActionProducerOptions>(configuration.GetSection(RoadmapWorkspaceActionProducerOptions.SectionName));
        services.AddSingleton<IRoadmapWorkspaceActionProducer, RoadmapWorkplaceActionProducer>();

        services.AddSingleton<IWorkspaceEventsProcessor, WorkspaceEventsProcessor>();

        return services;
    }
}