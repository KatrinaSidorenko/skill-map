namespace LearningPlatform.Core.IntegrationTests.Engine.Configuration;
internal interface IDatabaseConfiguration
{
    Dictionary<string, string?> Get();
}
