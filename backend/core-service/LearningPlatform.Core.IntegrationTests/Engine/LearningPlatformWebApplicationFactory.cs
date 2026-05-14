using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace LearningPlatform.Core.IntegrationTests.Engine;
public class LearningPlatformWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder) => builder.ConfigureAppConfiguration(
        (_, configBuilder) =>
        {
            //var settingsPath = Path.Combine(
            //    Directory.GetCurrentDirectory(),
            //    "appsettings.IntegrationTests.json");

            //configBuilder.AddJsonFile(settingsPath);
        });
}