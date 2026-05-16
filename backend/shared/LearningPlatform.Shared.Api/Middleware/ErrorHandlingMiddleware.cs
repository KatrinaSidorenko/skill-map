using System.Net;
using System.Security.Authentication;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using SkillMap.Shared.Results;

namespace LearningPlatform.Shared.Api.Middleware;

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
            AuthenticationException _ => new ExceptionResponse(HttpStatusCode.Unauthorized, "Authentication failed."),
            UnauthorizedAccessException _ => new ExceptionResponse(HttpStatusCode.Forbidden, "Unauthorized."),

            ArgumentNullException _ => new ExceptionResponse(HttpStatusCode.BadRequest, "Application exception occurred."),
            KeyNotFoundException _ => new ExceptionResponse(HttpStatusCode.NotFound, "The request key not found."),
            ArgumentException argEx => new ExceptionResponse(HttpStatusCode.BadRequest, argEx.Message),
            LearningPlatformException lpe => new ExceptionResponse(lpe.ToStatusCode(), lpe.Message),
            InvalidOperationException invOpEx => new ExceptionResponse(HttpStatusCode.InternalServerError, invOpEx.Message),
            _ => new ExceptionResponse(HttpStatusCode.InternalServerError, "Internal server error.")
        };

        httpResponse.ContentType = "application/json";
        httpResponse.StatusCode = (int)response.statusCode;
        await httpResponse.WriteAsJsonAsync(response, ct);
    }
}