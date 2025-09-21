using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SkillMap.Business.Account;
using SkillMap.Business.Roadmaps;
using SkillMap.Business.UserRoadmaps;

namespace SkillMap.Business;

public static class LayerRegistration
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IUserRoadmapsService, UserRoadmapsService>();
        services.AddScoped<ICustomizedRoadmapsService, CustomizedRoadmapsService>();

        services.AddValidatorsFromAssemblies([typeof(IAccountService).Assembly]);

        return services;
    }
}
