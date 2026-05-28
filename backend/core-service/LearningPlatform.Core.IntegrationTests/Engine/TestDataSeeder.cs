using LearningPlatform.Core.IntegrationTests.Account;
using LearningPlatform.Core.IntegrationTests.Engine.MessageBroker;
using LearningPlatform.Core.IntegrationTests.RoadmapsWorkspace;

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

        await SeedDatabaseAsync(scope.ServiceProvider);
        await ProvisionKafkaTopicsAsync(scope.ServiceProvider, KafkaTopics.GetAllTopics().ToList()); // todo: refactor to pass it somehow
    }

    private static async Task SeedDatabaseAsync(IServiceProvider services)
    {
        var accountService = services.GetRequiredService<IAccountService>();
        var testUser = AccountParameters.GetValid();

        if (await accountService.UserExists(testUser.Id, CancellationToken.None))
            return;

        var testUserRequest = new UserRegistrationDto
        {
            UserName = testUser.UserName,
            Email = testUser.Email,
            Password = testUser.Password,
            Role = testUser.Role
        };

        await accountService.Register(testUserRequest, CancellationToken.None);
    }

    private static async Task ProvisionKafkaTopicsAsync(IServiceProvider services, List<KafkaTopicDescriptor> kafkaTopics)
    {
        var provisioner = services.GetService<IKafkaTopicProvisioner>();
        if (provisioner == null)
            return;

        var kafkaContainer = services.GetService<KafkaContainer>();
        if (kafkaContainer == null)
            return;

        await provisioner.EnsureTopicsExistAsync(kafkaContainer.BootstrapServers, kafkaTopics, CancellationToken.None);
    }
}
