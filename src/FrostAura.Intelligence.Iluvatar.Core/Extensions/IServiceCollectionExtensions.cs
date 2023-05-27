using FrostAura.Intelligence.Iluvatar.Core.Extensions.Cognitive;
using FrostAura.Intelligence.Iluvatar.Core.Skills;
using FrostAura.Intelligence.Iluvatar.Core.Skills.Cognitive;
using FrostAura.Intelligence.Iluvatar.Core.Skills.Core;
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
        public static IServiceCollection AddCoreConfiguration(this IServiceCollection serviceCollection, IConfigurationRoot configuration)
		{
			return serviceCollection;
        }

        /// <summary>
        /// Register core services to the DI service collection.
        /// </summary>
        /// <param name="serviceCollection">The DI service collection.</param>
        /// <returns>The DI service collection.</returns>
        public static IServiceCollection AddCoreServices(this IServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            var types = typeof(BaseSkill)
                .Assembly
                .GetAllSkillTypesInAssembly()
                .ToList();

            foreach (var type in types)
            {
                serviceCollection.AddSingleton(type);

                if (type != typeof(PlannerSkill) && type != typeof(LLMSkill))
                {
                    serviceCollection.AddSingleton(typeof(BaseSkill), type);
                }
            }

            return serviceCollection;
        }
    }
}
