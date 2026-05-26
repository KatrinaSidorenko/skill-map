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
public class CreateRoadmapWorkspaceSnapshotTests : IClassFixture<LearningPlatformWebApplicationFactory<SkillMap.Api.Program>>, IClassFixture<DatabaseContainer>
{
    private readonly WebApplicationFactory<SkillMap.Api.Program> _configuredFactory;
    private readonly HttpClient _applicationHttpClient;
    private readonly KafkaContainer _kafkaContainer;

    public CreateRoadmapWorkspaceSnapshotTests(
        LearningPlatformWebApplicationFactory<SkillMap.Api.Program> webApplicationFactory,
        DatabaseContainer databaseContainer,
        KafkaContainer kafkaContainer)
    {
        _kafkaContainer = kafkaContainer;
        _configuredFactory = webApplicationFactory
            .WithContainerDatabaseConfigured(new RoadmapsWorkspaceDatabaseConfiguration(databaseContainer.ConnectionString!))
            .WithOptionsConfiguration(new BuildRoadmapWorkspaceSnapshotConfiguration())
            .WithOptionsConfiguration(new WorkspaceActionsConsumerKafkaConfiguration(kafkaContainer.BootstrapServers))
            .WithOptionsConfiguration(new WorkspaceActionReviewedProducerKafkaConfiguration(kafkaContainer.BootstrapServers));

        _applicationHttpClient = _configuredFactory.CreateClient();
    }

    // seed workspace and empty snapshot with Initial Version (maybe can use logic from unit tests)
    // 0. create kaka testcontainer
    // create the topic for con RoadmapWorkspaceActionConsumerOptions and WorkspaceActionReviewedProducerOptions in appsetting json provided topic names with partions 1 and replication 1
    // 1. create kafka configuration (as for db) and repalce them for condumre of actions
    // 2. create mock actions 
    // 3. produce actions as in the RoadmapWorkspaceActionProducer
    // 4. than consumer should made its part and will get event and write to db
    // 5. check that events are in db and the version of events are correct
    // 6. also as separet test create events more than IntevalOfSnphoting braikpount and check of snapshot that is created with version that is % == 0 to the interval

}
