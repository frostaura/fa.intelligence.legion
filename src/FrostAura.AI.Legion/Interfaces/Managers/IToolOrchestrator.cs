using FrostAura.AI.Legion.Models.LanguageModels;

namespace FrostAura.AI.Legion.Interfaces.Managers;

/// <summary>
/// A tool orchestrator component. Typically leveraged to get a collection of supported tools and allow execution of them.
/// </summary>
public interface IToolOrchestrator
{
	/// <summary>
	/// Get a collection of the available tools.
	/// </summary>
	/// <param name="token">Token to cancel downstream operations.</param>
	/// <returns>A collection of the available tools.</returns>
	Task<List<Tool>> GetAvailableToolsAsync(CancellationToken token);
	/// <summary>
	/// Execute a given tool and return it's stringified response.
	/// </summary>
	/// <param name="tool">The tool to execute.</param>
	/// <param name="executionContext">The context which to execute the tool with.</param>
	/// <param name="token">Token to cancel downstream operations.</param>
	/// <returns>The string response from the tool's execution result.</returns>
	Task<string> ExecuteToolAsync(Tool tool, Ollama.ToolCall executionContext, CancellationToken token);
}
