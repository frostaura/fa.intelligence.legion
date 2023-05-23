using FrostAura.Intelligence.Iluvatar.Shared.Models.Config;
using FrostAura.Intelligence.Iluvatar.Telegram.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FrostAura.Intelligence.Iluvatar.Shared.Extensions
{
	/// <summary>
	/// Service collection extensions.
	/// </summary>
	public static class IServiceCollectionExtensions
	{
        /// <summary>
        /// Bind the required configuration to the DI service collection.
        /// </summary>
        /// <param name="serviceCollection">The DI service collection.</param>
        /// <returns>The DI service collection.</returns>
        public static IServiceCollection AddTelegramConfiguration(this IServiceCollection serviceCollection, IConfigurationRoot configuration)
		{
			return serviceCollection
                .Configure<TelegramConfig>(configuration.GetSection("Telegram"));
        }

        /// <summary>
        /// Register Telegram services to the DI service collection.
        /// </summary>
        /// <param name="serviceCollection">The DI service collection.</param>
        /// <returns>The DI service collection.</returns>
        public static IServiceCollection AddTelegramServices(this IServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            serviceCollection
                .AddLogging(builder =>
                {
                    builder.AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = true;
                    });
                });

            return serviceCollection
                .AddTransient<TelegramManager>();
        }
    }
}
