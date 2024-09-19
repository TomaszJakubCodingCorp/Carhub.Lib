using Carhub.Lib.Cqrs.Commands.Configuration;
using Carhub.Lib.Cqrs.Dispatchers.Configuration;
using Carhub.Lib.Cqrs.Queries.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Carhub.Lib.Cqrs.Configuration;

public static class Extensions
{
    public static IServiceCollection AddCarhubCqrs(this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        services
            .AddCommands(assemblies)
            .AddQueries(assemblies)
            .AddDispatchers();
        return services;
    }
}