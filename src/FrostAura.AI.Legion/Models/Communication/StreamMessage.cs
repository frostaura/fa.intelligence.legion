using FrostAura.AI.Legion.Enums.Communication;
using FrostAura.AI.Legion.Models.Common;

namespace FrostAura.AI.Legion.Models.Communication;

/// <summary>
/// A message for stream request.
/// </summary>
public class StreamMessage
{
	/// <summary>
	/// The unique identifier of the message.
	/// </summary>
	public Guid MessageId { get; } = Guid.NewGuid();
	/// <summary>
	/// The unique identifier of the conversation. This should be the same for a corrosponding request and response.
	/// </summary>
	public Guid ConversationId { get; set; } = Guid.NewGuid();
	/// <summary>
	/// The request context associated with the message.
	/// </summary>
	public MessageDirection Type { get; set; }
	/// <summary>
	/// The request context associated with the message.
	/// </summary>
	public LegionRequest Request { get; set; }
	/// <summary>
	/// The response context associated with the message.
	/// </summary>
	public LegionResponse? Response { get; set; }
}
