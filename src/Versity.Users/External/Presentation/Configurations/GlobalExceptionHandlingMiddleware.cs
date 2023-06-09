using System.Net;
using System.Text.Json;
using Application.Exceptions;

namespace Presentation.Configurations;

// TODO: Replace that
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
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

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.NoContent;
        var stackTrace = string.Empty;
        var message = string.Empty;
        var exceptionType = exception.GetType();

        if (exceptionType == typeof(NotFoundException<>))
        {
            message = exception.Message;
            statusCode = HttpStatusCode.NotFound;
            stackTrace = exception.StackTrace;
        }
        else if (exceptionType == typeof(BadRequestException))
        {
            message = exception.Message;
            statusCode = HttpStatusCode.BadRequest;
            stackTrace = exception.StackTrace;
        }
        else
        {
            message = exception.Message;
            statusCode = HttpStatusCode.InternalServerError;
            stackTrace = exception.StackTrace;
        }

        var exceptionResult = JsonSerializer.Serialize(new { error = message, stackTrace});
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(exceptionResult);
    }
}