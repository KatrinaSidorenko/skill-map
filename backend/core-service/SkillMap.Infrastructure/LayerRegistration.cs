using LearningPlatform.RoadmapTests.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapTest;
using SkillMap.Infrastructure.Account;
using SkillMap.Infrastructure.Cache;
using SkillMap.Infrastructure.Email;
using SkillMap.Infrastructure.RoadmapTest;
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
        services.AddScoped<ICacheService, InMemoryCacheService>();
        services.AddMemoryCache();
        services.AddTransient<IEmailService, MailkitEmailService>();
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
        services.Configure<RoadmapTestingServiceOptions>(configuration.GetSection(RoadmapTestingServiceOptions.SectionName));

        services.AddHttpClient<IRoadmapTestGenerator, RoadmapTestGeneratorClient>(
            (sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<RoadmapTestingServiceOptions>>();
                client.BaseAddress = new Uri(options.Value.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            });

        return services;
    }
}
