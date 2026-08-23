using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Santa.Firebase.Services;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Santa.Firebase.Services with zero configuration.
    /// Automatically auto-detects 'Firebase' section in appsettings.json or 'firebase-credentials.json' in project root.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddSantaFirebaseServices(this IServiceCollection services)
    {
        services.AddOptions<FirebaseServiceOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                var section = configuration.GetSection("Firebase");
                if (section.Exists())
                {
                    section.Bind(options);
                }
            });

        services.AddSingleton<IFirebaseService, FirebaseService>();
        return services;
    }

    /// <summary>
    /// Registers Santa.Firebase.Services with custom configuration options.
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

