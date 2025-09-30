using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillMap.Business.Abstractions;
using SkillMap.Infrastructure.Account;
using SkillMap.Infrastructure.Cache;
using SkillMap.Infrastructure.Email;
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

        return services;
    }
}
