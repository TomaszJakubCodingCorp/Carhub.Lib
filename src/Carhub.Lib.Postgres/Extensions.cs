using Carhub.Lib.Configuration;
using Carhub.Lib.Postgres.Options;
using Microsoft.EntityFrameworkCore;
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
}