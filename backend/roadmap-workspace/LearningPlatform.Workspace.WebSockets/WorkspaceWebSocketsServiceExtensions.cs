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

        services.Configure<WorkspaceActionReviewedProducerOptions>(
        configuration.GetSection(WorkspaceActionReviewedProducerOptions.SectionName));

        services.Configure<WorkspaceActionReviewedConsumerOptions>(
        configuration.GetSection(WorkspaceActionReviewedConsumerOptions.SectionName));
        services.AddHostedService<WorkspaceActionReviewedConsumer>();

        return services;
    }
}
