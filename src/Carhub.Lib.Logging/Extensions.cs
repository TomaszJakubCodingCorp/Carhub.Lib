using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Carhub.Lib.Logging;

public static class Extensions
{
    public static void AddCarhubLogging(this WebApplicationBuilder builder, string serviceName)
    {
        var logFilePath = $"logs/log_{serviceName}_{DateTime.UtcNow:yyyy-MM-dd}.txt";

        builder.Host.UseSerilog((ctx, lc) => lc
            .ReadFrom.Configuration(ctx.Configuration)
            .WriteTo.Console()
            .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day));
    }
}