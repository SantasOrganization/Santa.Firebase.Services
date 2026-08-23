using System;
using Microsoft.Extensions.DependencyInjection;

namespace Santa.Firebase.Services;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Santa.Firebase.Services with the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="configureOptions">Delegate to configure <see cref="FirebaseServiceOptions"/>.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddSantaFirebaseServices(
        this IServiceCollection services,
        Action<FirebaseServiceOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddSingleton<IFirebaseService, FirebaseService>();
        return services;
    }
}
