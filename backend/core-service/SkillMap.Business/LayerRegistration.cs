using LearningPlatform.Roadmap.Business;

using Microsoft.Extensions.DependencyInjection;

using SkillMap.Application.Services;
using SkillMap.Business.Account;
using SkillMap.Business.Roadmaps;
using SkillMap.Business.RoadmapTest;
using SkillMap.Business.UserRoadmaps;
using SkillMap.Business.UserTest;

namespace SkillMap.Business;

public static class LayerRegistration
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IUserRoadmapsService, UserRoadmapsService>();
        services.AddScoped<ICustomizedRoadmapsService, CustomizedRoadmapsService>();
        services.AddScoped<IUserRoadmapTestService, UserRoadmapTestService>();
        services.AddRoadmapTestModule();
        services.AddRoadmapModule();

        return services;
    }
}