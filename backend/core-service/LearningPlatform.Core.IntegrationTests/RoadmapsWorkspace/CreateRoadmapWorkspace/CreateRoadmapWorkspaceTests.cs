using LearningPlatform.Core.IntegrationTests.Engine;
using LearningPlatform.Core.IntegrationTests.Engine.Configuration;
using LearningPlatform.Core.IntegrationTests.Engine.Database;

using SkillMap.Api;

namespace LearningPlatform.Core.IntegrationTests.RoadmapsWorkspace.CreateRoadmapWorkspace;

internal class CreateRoadmapWorkspaceTests(LearningPlatformWebApplicationFactory<Program> webApplicationFactory, DatabaseContainer databaseContainer) 
    : IClassFixture<LearningPlatformWebApplicationFactory<Program>>, IClassFixture<DatabaseContainer>
{
    private readonly HttpClient _applicationHttpClient = webApplicationFactory
        .WithContainerDatabaseConfigured(new RoadmapsWorkspaceDatabaseConfiguration(databaseContainer.ConnectionString!))
        .CreateClient(); // here is TestServer bootstrapped


}
