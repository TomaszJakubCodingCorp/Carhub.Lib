using Carhub.Lib.Cqrs.Dispatchers.Abstractions;
using Carhub.Lib.Cqrs.Dispatchers.Internals;
using Microsoft.Extensions.DependencyInjection;

namespace Carhub.Lib.Cqrs.Dispatchers.Configuration;

internal static class Extensions
{
    internal static IServiceCollection AddDispatchers(this IServiceCollection services)
        => services.AddSingleton<ICqrsDispatcher, CqrsDispatcher>();
}