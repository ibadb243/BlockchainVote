using FluentValidation;
using System.Net;
using System.Text.Json;
using WebAPI.Models;

namespace WebAPI.Middleware;

public class CustomExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public CustomExceptionHandlerMiddleware(RequestDelegate next)
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

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = new ErrorResponse
        {
            status = 400,
            message = string.Empty,
            errors = new Dictionary<string, string>()
        };

        switch (exception)
        {
            case JsonException:
                response.status = 400;
                response.message = "Validation failed";
                response.errors["input"] = "invalid format!";
                break;
            case ValidationException validationException:
                response.status = 400;
                response.message = "Validation failed";
                response.errors = validationException.Errors.ToDictionary(e => e.PropertyName, e => e.ErrorMessage);
                break;
        }
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.status;

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
