using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace LearningPlatform.Core.IntegrationTests.Engine;
public class LearningPlatformWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddScoped<ITestDataSeeder, TestDataSeeder>();
            services.AddAuthentication(TestAuthHandler.Schema)
                .AddScheme<AuthenticationSchemeOptions,
                    TestAuthHandler>(
                        TestAuthHandler.Schema,
                        _ => { });
        });

        builder.ConfigureTestServices(services =>
        {
            services.PostConfigureAll<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.Schema;
                options.DefaultChallengeScheme = TestAuthHandler.Schema;
            });
        });
    }
  
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<ITestDataSeeder>();
        seeder.SeedAsync(scope.ServiceProvider).GetAwaiter().GetResult();
        return host;
    }
}