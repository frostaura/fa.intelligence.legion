using FrostAura.Intelligence.Iluvatar.Shared.Models.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        public static IServiceCollection AddSharedConfiguration(this IServiceCollection serviceCollection, IConfigurationRoot configuration)
		{
			return serviceCollection
                .Configure<OpenAIConfig>(configuration.GetSection("OpenAI"))
                .Configure<PineconeConfig>(configuration.GetSection("Pinecone"));
        }

        /// <summary>
        /// Register shared services to the DI service collection.
        /// </summary>
        /// <param name="serviceCollection">The DI service collection.</param>
        /// <returns>The DI service collection.</returns>
        public static IServiceCollection AddSharedServices(this IServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            return serviceCollection;
        }
    }
}
