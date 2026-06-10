using FluentValidation;

using LearningPlatform.Roadmap.Business;
using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.Shared.Caching;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using SkillMap.Business.Abstractions;
using SkillMap.Business.Account;
using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Infrastructure.Account;
using SkillMap.Infrastructure.Database;
using SkillMap.Infrastructure.Email;
using SkillMap.Infrastructure.EventBus;
using SkillMap.Infrastructure.PersonalizedRoadmaps;
using SkillMap.Infrastructure.RoadmapAssessments;
using SkillMap.Infrastructure.RoadmapsWorkspace;
using SkillMap.Persistence;
using SkillMap.Persistence.Neo4j;
using SkillMap.Shared.Options;

namespace SkillMap.Infrastructure;

public static class LayerRegistration
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddJwt(configuration);

        services.AddScoped<IUserManager, UserManager>();
        services.AddScoped<ITokenService, TokenService>();

        services.Configure<PasswordHashOptions>(configuration.GetSection(PasswordHashOptions.SectionName));
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddScoped<IResetAccountService, ResetAccountService>();
        services.AddTransient<IEmailService, MailkitEmailService>();
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
        services.Configure<RoadmapTestingServiceOptions>(configuration.GetSection(RoadmapTestingServiceOptions.SectionName));

        services.AddHttpClient<IQuestionsGenerator, RoadmapAssessmentGeneratorClient>(
            (sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<RoadmapTestingServiceOptions>>();
                client.BaseAddress = new Uri(options.Value.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(60 * 2);
            });

        services.AddScoped<IAccountService, AccountService>();
        services.AddValidatorsFromAssemblies([typeof(IAccountService).Assembly]);

        services.AddRoadmapBlueprintModule();
        services.AddEventBus();

        // migrations
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddNeo4jPersistence(configuration);
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddPersistenceLayer(configuration);

        services.AddCaching();

        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseAutomaticMigrations();
        return app;
    }
}