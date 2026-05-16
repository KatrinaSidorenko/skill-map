
using LearningPlatform.Core.IntegrationTests.Engine.Configuration;

namespace LearningPlatform.Core.IntegrationTests.RoadmapsWorkspace;
internal class RoadmapsWorkspaceNeo4jDatabaseConfiguration : IDatabaseConfiguration
{
    private readonly string _connectionString;
    public RoadmapsWorkspaceNeo4jDatabaseConfiguration(string connectionString) => _connectionString = connectionString;
    public Dictionary<string, string?> Get()
    {
        return new Dictionary<string, string?>
        {
            { "ConnectionStrings:DefaultConnection", _connectionString}
        };
    }
}
