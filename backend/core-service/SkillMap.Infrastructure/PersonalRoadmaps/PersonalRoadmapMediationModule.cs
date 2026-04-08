using Microsoft.Extensions.DependencyInjection;

using SkillMap.Business.PersonalizedRoadmaps;

namespace SkillMap.Infrastructure.PersonalRoadmaps;
public static class PersonalRoadmapMediationModule
{
    public static IServiceCollection AddPersonalRoadmapsInfrastructure(this IServiceCollection services)
    {
        var commandsHandlersAssembly = typeof(IRoadmapWorkspaceModule).Assembly;
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblies(commandsHandlersAssembly));

        services.AddScoped<IRoadmapWorkspaceModule, PersonalRoadmapModule>();

        return services;
    }
}