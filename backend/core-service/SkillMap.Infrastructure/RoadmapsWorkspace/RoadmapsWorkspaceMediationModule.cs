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

        services.Configure<RoadmapWorkspaceActionConsumerOptions>(configuration.GetSection(RoadmapWorkspaceActionConsumerOptions.SectionName));
        services.AddHostedService<RoadmapWorkspaceActionConsumer>();

        services.Configure<WorkspaceActionReviewedProducerOptions>(
        configuration.GetSection(WorkspaceActionReviewedProducerOptions.SectionName));

        services.AddSingleton<IRoadmapWorkspaceActionReviewedNotifier, RoadmapWorkspaceActionReviewedNotifier>();
        services.AddSingleton<IWorkspaceEventsReviewer, RoadmapWorkspaceEventsReviewer>();

        //services.AddWorkspaceWebSockets(configuration);
        return services;
    }
}