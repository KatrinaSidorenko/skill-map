namespace LearningPlatform.Core.IntegrationTests.Engine;
internal class AsyncTestHelpers
{
    public static async Task Eventually(
        Func<Task> assertion,
        int timeoutMs = 30000,
        int pollIntervalMs = 500)
    {
        var timeout = DateTime.UtcNow.AddMilliseconds(timeoutMs);

        Exception? lastException = null;

        while (DateTime.UtcNow < timeout)
        {
            try
            {
                await assertion();
                return;
            }
            catch (Exception ex)
            {
                lastException = ex;
            }

            await Task.Delay(pollIntervalMs);
        }

        throw lastException ?? new TimeoutException();
    }
}
