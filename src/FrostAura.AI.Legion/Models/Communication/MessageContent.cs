using FrostAura.AI.Legion.Enums.Communication;

namespace FrostAura.AI.Legion.Models.Communication;

/// <summary>
/// The content of a message in the Legion system.
/// </summary>
public class MessageContent
{
	/// <summary>
	/// The actor responsible for producing the message content.
	/// </summary>
	public Actor ActorType { get; set; }
	/// <summary>
	/// The type of the content.
	/// </summary>
	public ContentType ContentType { get; set; }
	/// <summary>
	/// Content body of the message.
	/// </summary>
	public string Content { get; set; }
}
