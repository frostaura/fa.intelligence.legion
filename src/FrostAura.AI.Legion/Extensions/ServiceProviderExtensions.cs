using FrostAura.AI.Legion.Interfaces.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace FrostAura.AI.Legion.Extensions;

/// <summary>
/// Service provider extensions for the project.
/// </summary>
public static class ServiceProviderExtensions
{
	/// <summary>
	/// Get an instance of Legion.
	/// </summary>
	/// <param name="provider">The dependency container.</param>
	/// <returns>The Legion instance.</returns>
	public static ISemanticOrchestrator GetLegionInstance(this ServiceProvider provider)
	{
		throw new NotImplementedException();
	}
}
