using FrostAura.AI.Legion.Enums.Communication;
using FrostAura.AI.Legion.Extensions.LanguageModels;
using FrostAura.AI.Legion.Interfaces.Managers;
using FrostAura.AI.Legion.Models.LanguageModels;

namespace FrostAura.AI.Legion.Services.Managers;

/// <summary>
/// A tool orchestrator that consolidates various tool sources.
/// </summary>
public class GatewayToolOrchestrator : IToolOrchestrator
{
	/// <summary>
	/// Get a collection of the available tools.
	/// </summary>
	/// <param name="token">Token to cancel downstream operations.</param>
	/// <returns>A collection of the available tools.</returns>
	public Task<List<Tool>> GetAvailableToolsAsync(CancellationToken token)
	{
		var tools = new List<Tool>
		{
			new Tool
			{
				Name = "get_current_time",
				Description = "Get the current time in the user's locale.",
				/*Parameters = new List<Parameter>
				{
					new Parameter
					{
						Name = "days_of_forecast",
						Description = "Get the current time in the user's locale for a given period of time in days.",
						Type = ParameterType.Integer,
						Required = true
					}
				},*/
				Function = (args, token) =>
				{
					return Task.FromResult(DateTime.Now.ToString());
				}
			},
			new Tool
			{
				Name = "get_bank_balance",
				Description = "Get the current user's bank balance",
				Parameters = new List<Parameter>
				{
					new Parameter
					{
						Name = "bank_name",
						Description = "FNB, for example.",
						Type = ParameterType.Integer,
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
	public Task<string> ExecuteToolAsync(Tool tool, Ollama.ToolCall executionContext, CancellationToken token)
	{
		return tool.Function(executionContext.FromOllamaParameters(), token);
	}
}
