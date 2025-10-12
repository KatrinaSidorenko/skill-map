using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using SkillMap.Persistence.Neo4j.Helpers;
using SkillMap.Shared.Results;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace SkillMap.Persistence.Neo4j;

public abstract class BaseRepository
{
    protected IDriver Driver { get; }
    protected DbSettings DbSettings { get; }
    protected ILogger Logger { get; }
    public BaseRepository(IDriver driver, DbSettings dbSettings, ILogger logger)
    {
        Driver = driver ?? throw new ArgumentNullException(nameof(driver));
        DbSettings = dbSettings ?? throw new ArgumentNullException(nameof(dbSettings));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected async Task<Result<bool>> ExecuteCommands(List<Command> commands, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        using var session = Driver.AsyncSession(s => s.WithDatabase(DbSettings.Name));
        using var transaction = await session.BeginTransactionAsync();

        try
        {
            foreach (var command in commands)
            {
                await transaction.RunAsync(command.Text, command.Value);
            }

            await transaction.CommitAsync();

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to execute commands");
            return ResultType.FailedToSave<bool>(ex.Message);
        }
        finally
        {
            await session.CloseAsync();
        }
    }

    protected async Task<Result<bool>> ExecuteCommands(IAsyncTransaction transaction, List<Command> commands, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        try
        {
            foreach (var command in commands)
            {
                await transaction.RunAsync(command.Text, command.Value);
            }

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to execute commands");
            return ResultType.FailedToSave<bool>(ex.Message);
        }
    }
}
