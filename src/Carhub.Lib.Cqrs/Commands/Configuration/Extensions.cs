using System.Reflection;
using Carhub.Lib.Cqrs.Commands.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Carhub.Lib.Cqrs.Commands.Configuration;

internal static class Extensions
{
    internal static IServiceCollection AddCommands(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        return services;
    }
}