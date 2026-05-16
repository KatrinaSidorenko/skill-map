using LearningPlatform.Core.IntegrationTests.Account;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SkillMap.Business.Account;
using SkillMap.Business.Account.Models;

namespace LearningPlatform.Core.IntegrationTests.Engine;

public interface ITestDataSeeder
{
    Task SeedAsync(IServiceProvider services);
}
internal class TestDataSeeder : ITestDataSeeder
{
    public async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
        var testUser = AccountParameters.GetValid();
        if (await accountService.UserExists(testUser.Id, CancellationToken.None))
        {
            return;
        }
        var testUserRequest = new UserRegistrationDto
        {
            UserName = testUser.UserName,
            Email = testUser.Email,
            Password = testUser.Password,
            Role = testUser.Role
        };
        await accountService.Register(testUserRequest, CancellationToken.None);
    }
}
