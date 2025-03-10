using FrostAura.AI.Legion.Models.Communication;

namespace FrostAura.AI.Legion.Models.Common;

/// <summary>
/// A request for the Legion system.
/// </summary>
public class LegionRequest
{
	/// <summary>
	/// The request content
	/// </summary>
	public List<MessageContent> Content { get; set; } = new List<MessageContent>();
}
