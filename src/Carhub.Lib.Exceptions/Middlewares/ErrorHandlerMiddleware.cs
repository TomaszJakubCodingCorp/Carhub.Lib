using System.Net;
using Carhub.Lib.Exceptions.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Carhub.Lib.Exceptions.Middlewares;

internal sealed class ErrorHandlerMiddleware(
    ILogger<ErrorHandlerMiddleware> logger)
    : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Exception message: {Message}", exception.Message);
            await HandleErrorAsync(context, exception);
        }
    }

    private static async Task HandleErrorAsync(HttpContext context, Exception exception)
    {
        var errorResponse = Map(exception);
        context.Response.StatusCode = (int)errorResponse!.StatusCode;
        var response = errorResponse!.Errors;

        await context.Response.WriteAsJsonAsync(response);
    }

    private static ErrorsResponse Map(Exception exception)
    {
        return exception switch
        {
            CarhubApplicationException ex => new ErrorsResponse(HttpStatusCode.BadRequest,
                new Error(exception.GetType().Name, ex.Message)),
            CarhubDomainException ex => new ErrorsResponse(HttpStatusCode.BadRequest,
                new Error(exception.GetType().Name, ex.Message)),
            _ => new ErrorsResponse(HttpStatusCode.InternalServerError,
                new Error("error", "There was an error."))
        };
    }

    private sealed record ErrorsResponse(HttpStatusCode StatusCode, params Error[] Errors);

    private sealed record Error(string Code, string Message);
}