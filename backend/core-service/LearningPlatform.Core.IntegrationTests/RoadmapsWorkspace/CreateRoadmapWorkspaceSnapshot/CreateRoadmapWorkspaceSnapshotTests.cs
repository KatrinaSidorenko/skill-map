using System.Diagnostics.Tracing;
using System.Net.Http.Json;

using LearningPlatform.Core.IntegrationTests.Engine;
using LearningPlatform.Core.IntegrationTests.Engine.Configuration;
using LearningPlatform.Core.IntegrationTests.Engine.Database;
using LearningPlatform.Core.IntegrationTests.Engine.MessageBroker;
using LearningPlatform.Core.IntegrationTests.RoadmapsWorkspace.CreateRoadmapWorkspace;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using SkillMap.Api.Roadmaps;
using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Business.RoadmapsWorkspace.Common;
using SkillMap.Core.PersonalizedRoadmaps;

namespace LearningPlatform.Core.IntegrationTests.RoadmapsWorkspace.CreateRoadmapWorkspaceSnapshot;

public class CreateRoadmapWorkspaceSnapshotTests 
    :  IClassFixture<LearningPlatformWebApplicationFactory<SkillMap.Api.Program>>, 
    IClassFixture<DatabaseContainer>,
    IClassFixture<KafkaContainer>
{
    private readonly WebApplicationFactory<SkillMap.Api.Program> _configuredFactory;
    private readonly HttpClient _applicationHttpClient;
    private readonly KafkaContainer _kafkaContainer;

    public CreateRoadmapWorkspaceSnapshotTests(
        LearningPlatformWebApplicationFactory<SkillMap.Api.Program> webApplicationFactory,
        DatabaseContainer databaseContainer,
        KafkaContainer kafkaContainer)
    {
        _configuredFactory = webApplicationFactory
            .WithContainerDatabaseConfigured(new RoadmapsWorkspaceDatabaseConfiguration(databaseContainer.ConnectionString!))
            .WithOptionsConfiguration(new BuildRoadmapWorkspaceSnapshotConfiguration())
            .WithOptionsConfiguration(new WorkspaceActionsConsumerKafkaConfiguration(kafkaContainer.BootstrapServers))
            .WithOptionsConfiguration(new WorkspaceActionReviewedProducerKafkaConfiguration(kafkaContainer.BootstrapServers));

        _applicationHttpClient = _configuredFactory.CreateClient();
        _kafkaContainer = kafkaContainer;
    }

    [Fact]
    internal async Task Given_WorkspaceWithEventsBelowSnapshotInterval_When_EventsProduced_Then_EventsWrittenToDbWithCorrectVersion()
    {
        // Arrange
        var createWorkspaceParameters = CreateEmptyRoadmapWorkspaceParameters.GetValid();
        var createResponse = await _applicationHttpClient.PostAsJsonAsync(RoadmapsWorkspaceApiPaths.CreateEmptyRoadmapWorkspace, createWorkspaceParameters.GetRequest());

        createResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
        var workspaceId = await createResponse.Content.ReadFromJsonAsync<long>();

        var actions = WorkspaceActionFakers.FakeMixedActionSequence(workspaceId, startClientVersion: RoadmapWorkspaceConstants.InitialVersion);
        var eventsCount = actions.Count;
        var expectedLastVersion = WorkspaceSnapshotTestHelper.ExpectedLastVersion(eventsCount);

        // Act
        using var producer = new TestWorkspaceActionProducer(_kafkaContainer.BootstrapServers, KafkaTopics.WorkspaceActions);
        await producer.PublishManyAsync(actions);

        // Assert
        using var scope = _configuredFactory.Services.CreateScope();
        var eventRepository = scope.ServiceProvider.GetRequiredService<IRoadmapWorkspaceEventRepository>();

        List<RoadmapWorkspaceEvent> events = [];
        await AsyncTestHelpers.Eventually(async () =>
        {
            events = await eventRepository.GetEventsAfter(workspaceId, RoadmapWorkspaceConstants.InitialVersion, CancellationToken.None);
            events.Count.ShouldBe(eventsCount);
        });

        var lastEvent = events.MaxBy(e => e.Version)!;
        lastEvent.Version.ShouldBe(expectedLastVersion);
    }
}
