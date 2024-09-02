using System.Reflection;
using Carhub.Lib.Cqrs.Queries.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Carhub.Lib.Cqrs.Queries.Configuration;

internal static class Extensions
{
    internal static IServiceCollection AddQueries(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        return services;
    }
}