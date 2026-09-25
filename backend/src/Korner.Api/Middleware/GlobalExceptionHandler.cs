using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Sentry;

namespace Korner.Api.Middleware;

/// <summary>
/// Catches anything that escapes controllers/services and turns it into a 500 ProblemDetails
/// response with a stable code — never the exception message or stack trace (they go to the
/// log and Sentry only).
/// </summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception");
        SentrySdk.CaptureException(exception);

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
        };
        problem.Extensions["code"] = "Server.UnexpectedError";

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken: cancellationToken);

        return true;
    }
}
