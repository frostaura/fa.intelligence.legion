using Ollama;

namespace FrostAura.AI.Legion.Models.Common;

/// <summary>
/// A request for the Legion system.
/// </summary>
public class LegionRequest
{
	/// <summary>
	/// The request content
	/// </summary>
	public List<Message> Content { get; set; } = new List<Message>();
}
