using System.Text.Json;

namespace CareerAI.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedAccessException exception)
        {
            _logger.LogWarning(
                exception,
                "Unauthorized request.");

            await WriteResponseAsync(
                context,
                StatusCodes.Status401Unauthorized,
                exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "An unhandled exception occurred.");

            var statusCode = exception.Message switch
            {
                "Email is already registered."
                    => StatusCodes.Status400BadRequest,

                "Invalid registration role."
                    => StatusCodes.Status400BadRequest,

                "Invalid email or password."
                    => StatusCodes.Status401Unauthorized,

                "Job not found."
                    => StatusCodes.Status404NotFound,

                "Application not found."
                    => StatusCodes.Status404NotFound,

                "Application deadline has passed."
                    => StatusCodes.Status400BadRequest,

                "You have already applied for this job."
                    => StatusCodes.Status400BadRequest,

                "You have already saved this job."
                    => StatusCodes.Status400BadRequest,

                "Invalid application status."
                    => StatusCodes.Status400BadRequest,

                _ => StatusCodes.Status500InternalServerError
            };

            var message =
                statusCode == StatusCodes.Status500InternalServerError
                    ? "An unexpected error occurred."
                    : exception.Message;

            await WriteResponseAsync(
                context,
                statusCode,
                message);
        }
    }

    private static async Task WriteResponseAsync(
        HttpContext context,
        int statusCode,
        string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            statusCode,
            message
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}