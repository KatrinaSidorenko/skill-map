using Microsoft.AspNetCore.Mvc.Testing;

namespace LearningPlatform.Core.IntegrationTests.Engine.Configuration;
internal static class ConfigurationExtensions
{
    public static WebApplicationFactory<T> WithOptionsConfiguration<T>(
        this WebApplicationFactory<T> webApplicationFactory, IOptionsConfiguration configuration)
        where T : class => webApplicationFactory.UseSettings(configuration.Get());
    public static WebApplicationFactory<T> WithContainerDatabaseConfigured<T>(
        this WebApplicationFactory<T> webApplicationFactory, IDatabaseConfiguration databaseConfiguration)
        where T : class => WithOptionsConfiguration(webApplicationFactory, databaseConfiguration);
    private static WebApplicationFactory<T> UseSettings<T>(
        this WebApplicationFactory<T> webApplicationFactory,
        Dictionary<string, string?> settings)
        where T : class =>
        webApplicationFactory.WithWebHostBuilder(webHostBuilder =>
        {
            foreach (var setting in settings)
            {
                webHostBuilder.UseSetting(setting.Key, setting.Value);
            }
        });
}
