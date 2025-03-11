using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SpringBoard.Domain.Exceptions;

namespace SpringBoard.Api.Middleware;

/// <summary>
/// Middleware for handling exceptions globally and converting them to standardized API responses.
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalExceptionHandlerMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="environment">The hosting environment.</param>
    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var statusCode = GetStatusCode(exception);
        var errorCode = GetErrorCode(exception);

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = GetErrorTitle(statusCode),
            Detail = GetUserFriendlyMessage(exception),
            Instance = context.Request.Path,
            Type = $"https://springboard.example.com/errors/{errorCode.ToLowerInvariant()}"
        };

        // Add custom properties
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;
        problemDetails.Extensions["errorCode"] = errorCode;

        if (_environment.IsDevelopment())
        {
            problemDetails.Extensions["exception"] = exception.ToString();
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(problemDetails, options);
        await context.Response.WriteAsync(json);
    }

    private static HttpStatusCode GetStatusCode(Exception exception)
    {
        return exception switch
        {
            EntityNotFoundException => HttpStatusCode.NotFound,
            DomainUnauthorizedException => HttpStatusCode.Forbidden,
            InvalidEntityStateException => HttpStatusCode.BadRequest,
            DuplicateEntityException => HttpStatusCode.Conflict,
            DomainException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };
    }

    private static string GetErrorCode(Exception exception)
    {
        return exception switch
        {
            EntityNotFoundException => "ENTITY_NOT_FOUND",
            DomainUnauthorizedException => "UNAUTHORIZED",
            InvalidEntityStateException => "INVALID_ENTITY_STATE",
            DuplicateEntityException => "DUPLICATE_ENTITY",
            DomainException => "DOMAIN_ERROR",
            _ => "INTERNAL_SERVER_ERROR"
        };
    }

    private static string GetUserFriendlyMessage(Exception exception)
    {
        // For domain exceptions, use the exception message as it should be user-friendly
        if (exception is DomainException)
        {
            return exception.Message;
        }

        // For other exceptions, provide a generic message
        return "An unexpected error occurred. Please try again later.";
    }

    private static string GetErrorTitle(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.NotFound => "Resource Not Found",
            HttpStatusCode.BadRequest => "Bad Request",
            HttpStatusCode.Forbidden => "Forbidden",
            HttpStatusCode.Conflict => "Conflict",
            HttpStatusCode.InternalServerError => "Server Error",
            _ => "Error"
        };
    }
}
