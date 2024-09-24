using Carhub.Lib.MessageBrokers.RabbitMq.Conventions.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Carhub.Lib.MessageBrokers.RabbitMq.Conventions.Configuration;

internal static class Extensions
{
    internal static IServiceCollection AddConventions(this IServiceCollection services)
        => services
            .AddSingleton<IConventionProvider, ConventionProvider>();
}