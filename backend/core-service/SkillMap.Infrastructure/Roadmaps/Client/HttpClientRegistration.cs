using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillMap.Business.Abstractions;

namespace SkillMap.Infrastructure.Roadmaps.Client;

public static class HttpClientRegistration
{
    public const string RoadmapsBaseClient = "roadmaps-catalog";
    public static IServiceCollection AddRoadmapsCatalogHttpClientRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        var baseUrl = configuration.GetSection("RoadmapsCatalogServiceConfig").GetValue<string>("BaseUrl");

        services.AddHttpClient<IRoadmapsCatalogHttpClient, RoadmapsCatalogHttpClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        });


        return services;
    }
}
