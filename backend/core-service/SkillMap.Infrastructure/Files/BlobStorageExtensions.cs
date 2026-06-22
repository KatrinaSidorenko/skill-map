using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SkillMap.Infrastructure.Files;
internal static class BlobStorageExtensions
{
    internal static IServiceCollection AddBlobStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AzureBlobStorageOptions>(configuration.GetSection(AzureBlobStorageOptions.SectionName));
        services.AddScoped<IBlobStorageService, AzureBlobStorageService>();

        return services;
    }
}

