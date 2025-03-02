using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode statusCode;
        string title;

        switch (exception)
        {
            case ValidationException:
            case ArgumentException:
                statusCode = HttpStatusCode.BadRequest; 
                title = "Invalid Argument"; 
                break;
            case OperationCanceledException:
                statusCode = HttpStatusCode.RequestTimeout; 
                title = "Operation Timeout Error";
                break;
            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                title = "Resource Not Found";
                break;
            case InvalidOperationException:
                statusCode = HttpStatusCode.BadRequest;
                title = "Invalid Operation";
                break;
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized; 
                title = "Access Denied"; 
                break;
            case SecurityTokenException :
                statusCode = HttpStatusCode.Unauthorized; 
                title = "Invalid Security Token"; 
                break;
            case Exception:
                statusCode = HttpStatusCode.NotFound;
                title = "Resource Not Found";
                break;
            default:
                statusCode = HttpStatusCode.InternalServerError; 
                title = "An unexpected error occurred.";
                break;
        }
        
        var response = new 
        {
            Title = title,
            Status = (int)statusCode,
            Detail = exception.Message,
            StackTrace = _env.IsDevelopment() ? exception.StackTrace : null
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var jsonOptions = new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _env.IsDevelopment()
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, jsonOptions));
    }
}