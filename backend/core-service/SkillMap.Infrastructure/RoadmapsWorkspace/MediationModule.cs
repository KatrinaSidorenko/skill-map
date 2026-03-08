using Microsoft.Extensions.DependencyInjection;

using SkillMap.Business.PersonalizedRoadmaps;

namespace SkillMap.Infrastructure.PersonalizedRoadmaps;
internal static class MediationModule
{
    internal static IServiceCollection AddPersonalizedRoadmapModule(this IServiceCollection services)
    {
        var commandsHandlersAssembly = typeof(IRoadmapWorkspaceModule).Assembly;

        services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblies(commandsHandlersAssembly));

        services.AddScoped<IRoadmapWorkspaceModule, PersonalizedRoadmapModule>();

        return services;
    }
}
