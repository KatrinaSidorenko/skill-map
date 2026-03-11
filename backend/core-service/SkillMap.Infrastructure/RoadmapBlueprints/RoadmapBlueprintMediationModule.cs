using Microsoft.Extensions.DependencyInjection;

using SkillMap.Business.RoadmapBlueprints;

namespace SkillMap.Infrastructure.RoadmapBlueprints;

public static class RoadmapBlueprintMediationModule
{
    public static IServiceCollection AddRoadmapBlueprintsInfrastructure(this IServiceCollection services)
    {
        var commandsHandlersAssembly = typeof(IRoadmapBlueprintModule).Assembly;
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblies(commandsHandlersAssembly));

 services.AddScoped<IRoadmapBlueprintModule, RoadmapBlueprintModule>();

        return services;
    }
}
