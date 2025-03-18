using System.Text.Json;
using FrostAura.AI.Legion.Enums.Communication;
using FrostAura.AI.Legion.Extensions.LanguageModels;
using FrostAura.AI.Legion.Interfaces.Data;
using FrostAura.AI.Legion.Interfaces.Managers;
using FrostAura.AI.Legion.Models.LanguageModels;
using FrostAura.Libraries.Core.Extensions.Validation;
using Microsoft.Extensions.Logging;

namespace FrostAura.AI.Legion.Services.Managers;

/// <summary>
/// A tool orchestrator that consolidates various tool sources.
/// </summary>
public class GatewayToolOrchestrator : IToolOrchestrator
{
	/// <summary>
	/// The large language model instance.
	/// </summary>
	private readonly ILargeLanguageModel _largeLanguageModel;
	/// <summary>
	/// Instance logger.
	/// </summary>
	private readonly ILogger _logger;

	/// <summary>
	/// Overloaded constructor for injecting dependencies.
	/// </summary>
	/// <param name="largeLanguageModel">The large language model instance.</param>
	/// <param name="logger">Instance logger.</param>
	public GatewayToolOrchestrator(ILargeLanguageModel largeLanguageModel, ILogger<GatewayToolOrchestrator> logger)
	{
		_logger = logger.ThrowIfNull(nameof(logger));
		_largeLanguageModel = largeLanguageModel.ThrowIfNull(nameof(largeLanguageModel));
	}

	/// <summary>
	/// Get a collection of the available tools.
	/// </summary>
	/// <param name="token">Token to cancel downstream operations.</param>
	/// <returns>A collection of the available tools.</returns>
	public Task<List<Tool>> GetAvailableToolsAsync(CancellationToken token)
	{
		_logger.LogDebug("[{ClassName}][{MethodName}] Fetching all available tools.", nameof(LegionOrchestrator), nameof(GetAvailableToolsAsync));

		var tools = new List<Tool>
		{
			new Tool
			{
				Name = "get_current_time",
				Description = "Get the current time in the user's locale.",
				Function = (args, token) =>
				{
					return Task.FromResult(DateTime.Now.ToString());
				}
			},
			new Tool
			{
				Name = "tree_planner",
				Description = "Decompose a complex problem in a tree-like approach, solve for each node and finally return the response.",
				Parameters = new List<Parameter>
				{
					new Parameter
					{
						Name = "query",
						Description = "The query for the problem.",
						Type = ParameterType.String,
						Required = true
					}
				},
				Function = (args, token) =>
				{
					return Task.FromResult("$ 520.23");
				}
			}
		};

		return Task.FromResult(tools);
	}

	/// <summary>
	/// Execute a given tool and return it's stringified response.
	/// </summary>
	/// <param name="tool">The tool to execute.</param>
	/// <param name="executionContext">The context which to execute the tool with.</param>
	/// <param name="token">Token to cancel downstream operations.</param>
	/// <returns>The string response from the tool's execution result.</returns>
	public async Task<string> ExecuteToolAsync(Tool tool, Ollama.ToolCall executionContext, CancellationToken token)
	{
		var parameters = executionContext.FromOllamaParameters();

		_logger.LogDebug("[{ClassName}][{MethodName}] Executing tool '{ToolName}' with args: {ToolArgs}.", nameof(LegionOrchestrator), nameof(GetAvailableToolsAsync), tool.Name, JsonSerializer.Serialize(parameters));

		var response = await tool.Function(parameters, token);

		_logger.LogDebug("[{ClassName}][{MethodName}] Tool '{ToolName}' executed successfully. Response: {ToolResponse}", nameof(LegionOrchestrator), nameof(GetAvailableToolsAsync), tool.Name, response);

		return response;
	}
}
