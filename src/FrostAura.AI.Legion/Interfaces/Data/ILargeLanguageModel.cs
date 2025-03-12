using Ollama;

namespace FrostAura.AI.Legion.Interfaces.Data;

/// <summary>
/// An interface for a large language model that is platform-agnostic.
/// </summary>
public interface ILargeLanguageModel
{
	/// <summary>
	/// Initiate a chat with the large language model.
	/// </summary>
	/// <param name="messages">A collection of previous messages.</param>
	/// <param name="tools">Tools to expose to the language model. An empty array is acceptable.</param>
	/// <param name="token">Token to cancel downstream operations.</param>
	/// <returns>The new conversation history with the latest resonse as the latest response.</returns>
	Task<List<Message>> ChatAsync(List<Message> messages, List<Tool> tools, CancellationToken token);
	/// <summary>
	/// Embed text to a vector collection with an embedding model.
	/// </summary>
	/// <param name="text"></param>
	/// <param name="token">Token to cancel downstream operations.</param>
	/// <returns>Returns the collection of embedding vectors.</returns>
	Task<List<double>> EmbedAsync(string text, CancellationToken token);
}
