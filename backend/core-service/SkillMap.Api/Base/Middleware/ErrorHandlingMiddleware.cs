using SkillMap.Shared.Results;
using System.Net;

namespace SkillMap.Api.Base.Middleware;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            await WriteErrorToResponseAsync(context.Response, e, logger, $"{context.Request.Method}: {context.Request.Path}", context.RequestAborted);
        }
    }

    private static async Task WriteErrorToResponseAsync(HttpResponse httpResponse, Exception exception, ILogger logger, string actionName, CancellationToken ct = default)
    {
        logger.LogError(exception, $"An unexpected error occurred at {actionName}.");

        ExceptionResponse response = exception switch
        {
            ArgumentNullException _ => new ExceptionResponse(HttpStatusCode.BadRequest, "Application exception occurred."),
            KeyNotFoundException _ => new ExceptionResponse(HttpStatusCode.NotFound, "The request key not found."),
            UnauthorizedAccessException _ => new ExceptionResponse(HttpStatusCode.Unauthorized, "Unauthorized."),
            LearningPlatformException lpe => new ExceptionResponse(lpe.ToStatusCode(), lpe.Message),
            _ => new ExceptionResponse(HttpStatusCode.InternalServerError, "Internal server error.")
        };

        httpResponse.ContentType = "application/json";
        httpResponse.StatusCode = (int)response.StatusCode;
        await httpResponse.WriteAsJsonAsync(response, ct);
    }
}
