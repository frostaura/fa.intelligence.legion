using System.ComponentModel.DataAnnotations;
using FrostAura.AI.Legion.Enums.Communication;

namespace FrostAura.AI.Legion.Models.LanguageModels;

/// <summary>
/// A parameter representation of an argument that a tool may leverage.
/// </summary>
public class Parameter
{
	/// <summary>
	/// The parameter's unique name.
	/// </summary>
	[Required(ErrorMessage = "A valid parameter name is required.")]
	public string Name { get; set; }
	/// <summary>
	/// The tool's description which gets leveraged by the language model to decide on which tool to execute.
	/// </summary>
	[Required(ErrorMessage = "A valid description name is required.")]
	public string Description { get; set; }
	/// <summary>
	/// The type of the parameter.
	/// </summary>
	public ParameterType Type { get; set; } = ParameterType.String;
	/// <summary>
	/// Whether the parameter is required.
	/// </summary>
	public bool Required { get; set; } = false;
}
