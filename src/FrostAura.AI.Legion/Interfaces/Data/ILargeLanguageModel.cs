using FrostAura.AI.Legion.Models.Communication;

namespace FrostAura.AI.Legion.Interfaces.Data;

/// <summary>
/// An interface for a large language model that is platform-agnostic.
/// </summary>
public interface ILargeLanguageModel
{
	/// <summary>
	/// Initiate a chat with the large language model.
	/// </summary>
	/// <param name="conversationHistory">A collection of previous messages.</param>
	/// <param name="token">Token to cancel downstream operations.</param>
	/// <returns>The new conversation history with the latest resonse as the latest response.</returns>
	Task<List<MessageContent>> ChatAsync(List<MessageContent> conversationHistory, CancellationToken token);
}
