using Carhub.Lib.Postgres.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Carhub.Lib.Postgres;

public static class Extensions
{
    public static IServiceCollection AddPostgres<T>(this IServiceCollection services)
        where T : DbContext
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        var options = services.GetOptions<PostgresOptions>(PostgresOptions.OptionsName);
        services.AddDbContext<T>(x => x.UseNpgsql(options.ConnectionString));
        return services;
    }

    private static T GetOptions<T>(this IServiceCollection services, string sectionName)
        where T : new()
    {
        using var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        return configuration.GetOptions<T>(sectionName);
    }

    private static T GetOptions<T>(this IConfiguration configuration, string sectionName)
        where T : new()
    {
        var options = new T();
        configuration.GetSection(sectionName).Bind(options);
        return options;
    }
}