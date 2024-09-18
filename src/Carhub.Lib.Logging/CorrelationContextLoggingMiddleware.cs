using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Carhub.Lib.Logging;

public sealed class CorrelationContextLoggingMiddleware(ILogger<CorrelationContextLoggingMiddleware> logger) :
    IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var headers = Activity.Current?.Baggage
            .ToDictionary(x => x.Key, x => x.Value);

        if (headers is null)
            return next(context);

        using (logger.BeginScope(headers))
        {
            return next(context);
        }
    }
}