using Microsoft.Extensions.DependencyInjection;

using SkillMap.Business.PersonalizedRoadmaps;

namespace SkillMap.Infrastructure.PersonalizedRoadmaps;
internal static class MediationModule
{
    internal static IServiceCollection AddPersonalizedRoadmapModule(this IServiceCollection services)
    {
        var commandsHandlersAssembly = typeof(IPersonalizedRoadmapModule).Assembly;

        services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblies(commandsHandlersAssembly));

        services.AddScoped<IPersonalizedRoadmapModule, PersonalizedRoadmapModule>();

        return services;
    }
}
