using System.Text.Json;
using FrostAura.AI.Legion.Enums.Communication;
using Ollama;

namespace FrostAura.AI.Legion.Extensions.LanguageModels;

/// <summary>
/// A collection of extensions for tools.
/// </summary>
public static class ToolsExtensions
{
	/// <summary>
	/// Convert a collection of tools to an Ollama tools representation.
	/// </summary>
	/// <param name="tools">Collection of tools.</param>
	/// <returns>A collection of Ollama-compatible tools.</returns>
	public static List<Tool> AsOllamaTools(this List<Legion.Models.LanguageModels.Tool> tools)
	{
		var ollamaTools = new List<Tool>();

		foreach (var tool in tools)
		{
			var properties = new Dictionary<string, object>();

			foreach (var prop in tool.Parameters)
			{
				properties[prop.Name] = new
				{
					type = Enum.GetName(typeof(ParameterType), prop.Type).ToLower(),
					description = prop.Description
				};
			}

			var requiredProps = tool
				.Parameters
				.Where(p => p.Required)
				.Select(p => p.Name)
				.ToList();
			var parameters = new
			{
				type = "object",
				properties = properties,
				required = requiredProps
			};
			var ollamaToolFunction = new ToolFunction
			{
				Name = tool.Name,
				Description = tool.Description,
				Parameters = JsonSerializer.Serialize(parameters)
			};

			ollamaTools.Add(new Tool(ToolType.Function, ollamaToolFunction));
		}

		return ollamaTools;
	}

	/// <summary>
	/// Convert Ollama tool call parameters to Legion-specific ones.
	/// </summary>
	/// <param name="toolCall">The Ollama tool call context.</param>
	/// <returns>A collection of parsed parameters.</returns>
	public static Dictionary<string, object> FromOllamaParameters(this ToolCall toolCall)
	{
		var parsedParams = JsonSerializer.Deserialize<Dictionary<string, object>>(toolCall
			.Function
			.Arguments
			.AsJson());


		return parsedParams;
	}
}
