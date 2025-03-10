using FrostAura.AI.Legion.Models.Common;

namespace FrostAura.AI.Legion.Interfaces.Managers;

/// <summary>
/// A semantic orchestrator component. Typically the entry point to a semantic application.
/// </summary>
public interface ISemanticOrchestrator
{
	/// <summary>
	/// Initiate a workload with the Legion system.
	/// </summary>
	/// <param name="request">The request body for the workload.</param>
	/// <param name="token">Token to cancel downstream operations.</param>
	/// <returns>The response from the Legion system.</returns>
	Task<LegionResponse> ChatAsync(LegionRequest request, CancellationToken token);
	/// <summary>
	/// Initiate a workload with the Legion system.
	/// </summary>
	/// <param name="request">The text request body for the workload.</param>
	/// <param name="token">Token to cancel downstream operations.</param>
	/// <returns>The text response from the Legion system.</returns>
	Task<string> ChatAsync(string request, CancellationToken token);
}