using System.Diagnostics;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace LearningPlatform.Shared.Api.Middleware;

public class ExecutionTimeFilter : IActionFilter
{
    private Stopwatch _stopwatch;
    private readonly ILogger _logger;

    public void OnActionExecuting(ActionExecutingContext context, ILogger logger)
    {
        _stopwatch = Stopwatch.StartNew();
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {

    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _stopwatch.Stop();
        var time = _stopwatch.ElapsedMilliseconds;
        var actionName = context.ActionDescriptor.DisplayName;

        _logger.LogInformation($"Action {actionName} executed in {time} ms");
    }
}