using Microsoft.Extensions.Options;
using Npgsql;
using SkillMap.Shared.Options;
using System.Data;

namespace LearningPlatform.RoadmapTests.Service.Persistence;

public interface IDatabaseConnectionFactory
{
    Task<IDbConnection> CreateOpenConnectionAsync(CancellationToken ct);
}

public sealed class DatabaseConnectionFactory : IDatabaseConnectionFactory
{
    private readonly DatabaseConnectionOptions _options;

    public DatabaseConnectionFactory(
        IOptions<DatabaseConnectionOptions> options)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        _options = options.Value
            ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<IDbConnection> CreateOpenConnectionAsync(
        CancellationToken ct)
    {
        var connectionString = _options.GetPostgresConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "PostgreSQL connection string is not configured");

        var connection = new NpgsqlConnection(connectionString);

        try
        {
            await connection.OpenAsync(ct);
            return connection;
        }
        catch
        {
            await connection.DisposeAsync();
            throw;
        }
    }
}

