using FrostAura.AI.Legion.Interfaces.Data;
using FrostAura.AI.Legion.Interfaces.Managers;
using FrostAura.AI.Legion.Models.Communication;
using FrostAura.AI.Legion.Services.Data;
using Microsoft.Extensions.DependencyInjection;

namespace FrostAura.AI.Legion.Extensions;

/// <summary>
/// Service collection extensions for bootstrapping the project.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Register required services for the project.
	/// </summary>
	/// <param name="services">The service collection.</param>
	/// <returns>The augmented service collection.</returns>
	public static IServiceCollection AddLegion(this IServiceCollection services)
	{
		return services
			.AddSingleton<ISemanticOrchestrator, LegionOrchestrator>()
			.AddSingleton<IStream<StreamMessage>, InMemoryStream>()
			.AddLogging();
	}
}
