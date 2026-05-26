using System.Net.Http.Json;

using LearningPlatform.Core.IntegrationTests.Engine;
using LearningPlatform.Core.IntegrationTests.Engine.Configuration;
using LearningPlatform.Core.IntegrationTests.Engine.Database;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using SkillMap.Api;
using SkillMap.Api.Roadmaps;
using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Business.RoadmapsWorkspace.Common;

namespace LearningPlatform.Core.IntegrationTests.RoadmapsWorkspace.CreateRoadmapWorkspace;

public sealed class CreateRoadmapWorkspaceTests : IClassFixture<LearningPlatformWebApplicationFactory<SkillMap.Api.Program>>, IClassFixture<DatabaseContainer>
{
    private readonly WebApplicationFactory<SkillMap.Api.Program> _configuredFactory;
    private readonly HttpClient _applicationHttpClient;

    public CreateRoadmapWorkspaceTests(
        LearningPlatformWebApplicationFactory<SkillMap.Api.Program> webApplicationFactory,
        DatabaseContainer databaseContainer)
    {
        _configuredFactory = webApplicationFactory
            .WithContainerDatabaseConfigured(new RoadmapsWorkspaceDatabaseConfiguration(databaseContainer.ConnectionString!))
            .WithOptionsConfiguration(new BuildRoadmapWorkspaceSnapshotConfiguration());

        _applicationHttpClient = _configuredFactory.CreateClient();
    }

    [Fact]
    internal async Task Given_ValidRequest_When_CreatingEmptyWorkspace_Then_ReturnsCreated()
    {
        // Arrange
        var createEmptyWorkspaceParameters = CreateEmptyRoadmapWorkspaceParameters.GetValid();

        // Act
        var response = await _applicationHttpClient.PostAsJsonAsync(RoadmapsWorkspaceApiPaths.CreateEmptyRoadmapWorkspace, createEmptyWorkspaceParameters.GetRequest());

        // Assert 
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
        var workspaceId = await response.Content.ReadFromJsonAsync<long>();

        using var scope = _configuredFactory.Services.CreateScope();
        var roadmapWorkspaceEditorRepository = scope.ServiceProvider.GetRequiredService<IRoadmapWorkspaceEditor>();
        var actualRoadmapWorkspace = await roadmapWorkspaceEditorRepository.GetActualRoadmapSnapshot(workspaceId, CancellationToken.None);
        actualRoadmapWorkspace.LearningItems.ShouldBeEmpty();
        actualRoadmapWorkspace.LearningItemsConnections.ShouldBeEmpty();
        actualRoadmapWorkspace.Version.ShouldBe(RoadmapWorkspaceConstants.InitialVersion);
    }

}
