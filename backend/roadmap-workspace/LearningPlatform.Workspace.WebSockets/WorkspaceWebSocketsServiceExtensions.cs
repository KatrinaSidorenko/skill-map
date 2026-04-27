using System.Text.Json;

using LearningPlatform.Workspace.WebSockets.Contracts;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LearningPlatform.Workspace.WebSockets;

public static class WorkspaceWebSocketsServiceExtensions
{
    public static IServiceCollection AddWorkspaceWebSockets(
          this IServiceCollection services,
          IConfiguration configuration)
    {
        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
        })
        .AddJsonProtocol(options =>
        {
            //options.PayloadSerializerSettings.TypeNameHandling = 
            //options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        services.Configure<RoadmapWorkspaceActionProducerOptions>(
        configuration.GetSection(RoadmapWorkspaceActionProducerOptions.SectionName));
        services.AddSingleton<IRoadmapWorkspaceActionProducer, RoadmapWorkspaceActionProducer>();


        services.Configure<WorkspaceActionReviewedConsumerOptions>(
        configuration.GetSection(WorkspaceActionReviewedConsumerOptions.SectionName));
        services.AddHostedService<WorkspaceActionReviewedConsumer>();

        return services;
    }
}
