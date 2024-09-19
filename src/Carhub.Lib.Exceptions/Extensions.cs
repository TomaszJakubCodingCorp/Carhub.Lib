﻿using Carhub.Lib.Exceptions.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Carhub.Lib.Exceptions;

public static class Extensions
{
    public static IServiceCollection AddErrorHandling(this IServiceCollection services)
    {
        services
            .AddScoped<ErrorHandlerMiddleware>();
        return services;
    }

    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorHandlerMiddleware>();
        return app;
    }
}