using System.ComponentModel.DataAnnotations;

namespace FrostAura.AI.Legion.Models.LanguageModels;

/// <summary>
/// A tool / function that a large language model may call.
/// </summary>
public class Tool
{
	/// <summary>
	/// The tool's unique name.
	/// </summary>
	[Required(ErrorMessage = "A valid tool name is required.")]
	public string Name { get; set; }
	/// <summary>
	/// The tool's description which gets leveraged by the language model to decide on which tool to execute.
	/// </summary>
	[Required(ErrorMessage = "A valid tool description is required.")]
	public string Description { get; set; }
	/// <summary>
	/// The collection of arguments for the function call.
	/// </summary>
	public List<Parameter> Parameters { get; set; } = new List<Parameter>();
	/// <summary>
	/// The action to invoke when the tool should be utilized.
	///
	/// Function Input:
	/// - The collection of parameters to execute the tool with.
	/// - A cancellation token to cancel downstream calls.
	/// 
	/// Expected Function output:
	/// - The task that produces a string result, allowing for async and parallel calling.
	/// </summary>
	[Required(ErrorMessage = "A valid function is required for the tool.")]
	public Func<Dictionary<string, object>, CancellationToken, Task<string>> Function { get; set; }
}
