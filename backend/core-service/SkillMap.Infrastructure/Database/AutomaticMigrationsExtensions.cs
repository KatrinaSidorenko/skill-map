using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SkillMap.Persistence;

namespace SkillMap.Infrastructure.Database;
internal static class AutomaticMigrationsExtensions
{
    internal static IApplicationBuilder UseAutomaticMigrations(this IApplicationBuilder applicationBuilder)
    {
        //using var scope = applicationBuilder.ApplicationServices.CreateScope();
        //var context = scope.ServiceProvider.GetRequiredService<SkillMapDbContext>();
        //context.Database.Migrate();

        return applicationBuilder;
    }
}