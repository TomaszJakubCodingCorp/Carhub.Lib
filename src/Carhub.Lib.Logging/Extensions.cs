using Carhub.Lib.Logging.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Filters;

namespace Carhub.Lib.Logging;

public static class Extensions
{
    public static WebApplicationBuilder AddCarhubLogging(this WebApplicationBuilder builder,
        LoggerOptions loggerOptions,
        Action<HostBuilderContext, LoggerConfiguration>? configure = null)
    {
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Host
            .UseSerilog((context, loggerConfiguration) =>
            {
                MapOptions(loggerOptions, loggerConfiguration, context.HostingEnvironment.EnvironmentName);
                configure?.Invoke(context, loggerConfiguration);
            });

        return builder;
    }

    private static void MapOptions(LoggerOptions loggerOptions,
        LoggerConfiguration loggerConfiguration, string environmentName)
    {
        LoggingLevelSwitch loggingLevelSwitch = new()
        {
            MinimumLevel = GetLogEventLevel(loggerOptions.Level)
        };

        loggerConfiguration.Enrich.FromLogContext()
            .MinimumLevel.ControlledBy(loggingLevelSwitch)
            .Enrich.With<UserIdEnricher>()
            .Enrich.WithProperty("Environment", environmentName)
            .Enrich.WithProperty("Application", loggerOptions.App.Service)
            .Enrich.WithProperty("Version", loggerOptions.App.Version);

        foreach (var (key, value) in loggerOptions.Tags)
        {
            loggerConfiguration.Enrich.WithProperty(key, value);
        }

        foreach (var (key, value) in loggerOptions.MinimumLevelOverrides)
        {
            var logLevel = GetLogEventLevel(value);
            loggerConfiguration.MinimumLevel.Override(key, logLevel);
        }

        loggerOptions.ExcludePaths.ToList().ForEach(p => loggerConfiguration.Filter
            .ByExcluding(Matching.WithProperty<string>("RequestPath", n => n.EndsWith(p))));

        loggerOptions.ExcludeProperties.ToList().ForEach(p => loggerConfiguration.Filter
            .ByExcluding(Matching.WithProperty(p)));

        Configure(loggerConfiguration, loggerOptions);
    }

    private static void Configure(LoggerConfiguration loggerConfiguration, LoggerOptions options)
    {
        var consoleOptions = options.Console;
        var seqOptions = options.Seq;
        var fileOptions = options.File;

        if (consoleOptions.Enabled)
            loggerConfiguration.WriteTo.Console();

        if (seqOptions.Enabled)
            loggerConfiguration.WriteTo.Seq(seqOptions.Url, apiKey: seqOptions.ApiKey);

        if (fileOptions.Enabled)
        {
            var logFilePath = $"logs/log_{options.App.Service}_{DateTime.UtcNow:yyyy-MM-dd}.txt";
            loggerConfiguration.WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day);
        }
    }

    private static LogEventLevel GetLogEventLevel(string level)
    {
        return Enum.TryParse<LogEventLevel>(level, true, out var logLevel) ? logLevel : LogEventLevel.Information;
    }

    public static IServiceCollection AddCorrelationContextLogging(this IServiceCollection services)
    {
        services.AddTransient<CorrelationContextLoggingMiddleware>();
        return services;
    }

    public static IApplicationBuilder UserCorrelationContextLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationContextLoggingMiddleware>();
        return app;
    }
}