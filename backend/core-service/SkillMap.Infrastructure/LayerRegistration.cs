using FluentValidation;

using LearningPlatform.Roadmap.Business;
using LearningPlatform.RoadmapTests.Contracts;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

//using SkillMap.Application.Services;
//using SkillMap.Business.__old.ModifiedRoadmaps;
//using SkillMap.Business.__old.UserRoadmaps;
using SkillMap.Business.Abstractions;
using SkillMap.Business.Account;
using SkillMap.Business.RoadmapTest;
using SkillMap.Business.UserTest;
using SkillMap.Infrastructure.Account;
using SkillMap.Infrastructure.Email;
using SkillMap.Infrastructure.EventBus;
using SkillMap.Infrastructure.PersonalizedRoadmaps;
using SkillMap.Infrastructure.RoadmapTest;
using SkillMap.Persistence;
using SkillMap.Shared.Options;

namespace SkillMap.Infrastructure;

public static class LayerRegistration
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddJwt(configuration);

        services.AddScoped<IUserManager, UserManager>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IResetAccountService, ResetAccountService>();
        services.AddTransient<IEmailService, MailkitEmailService>();
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
        services.Configure<RoadmapTestingServiceOptions>(configuration.GetSection(RoadmapTestingServiceOptions.SectionName));

        services.AddHttpClient<ITopicQuestionsGenerator, RoadmapTestGeneratorClient>(
            (sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<RoadmapTestingServiceOptions>>();
                client.BaseAddress = new Uri(options.Value.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(60 * 2);
            });

        services.AddScoped<IAccountService, AccountService>();
        services.AddValidatorsFromAssemblies([typeof(IAccountService).Assembly]);
        //services.AddScoped<IUserRoadmapsService, UserRoadmapsService>();
        //services.AddScoped<ICustomizedRoadmapsService, CustomizedRoadmapsService>();
        //services.AddScoped<IUserRoadmapTestService, UserRoadmapTestService>();
        services.AddRoadmapTestModule();
        services.AddRoadmapModule();

        services.AddEventBus();
        services.AddPersonalizedRoadmapModule();

        // migrations
        services.AddPersistenceLayer(configuration);

        return services;
    }
}