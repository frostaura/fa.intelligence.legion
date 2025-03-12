using FrostAura.AI.Legion.Interfaces.Data;
using FrostAura.Libraries.Core.Extensions.Validation;
using Microsoft.Extensions.Logging;
using Ollama;

namespace FrostAura.AI.Legion.Services.Data;

/// <summary>
/// A large language model implementation that connects to an Ollama server.
///
/// Reference: https://github.com/tryAGI/Ollama
/// </summary>
public class OllamaLargeLanguageModel : ILargeLanguageModel, IDisposable
{
	/// <summary>
	/// The Ollama client for comms with the Ollama server.
	/// </summary>
	private readonly OllamaApiClient _ollama;
	/// <summary>
	/// Instance logger.
	/// </summary>
	private readonly ILogger _logger;

	/// <summary>
	/// Overloaded constructor for injecting dependencies.
	/// </summary>
	/// <param name="logger">Instance logger.</param>
	public OllamaLargeLanguageModel(ILogger<OllamaLargeLanguageModel> logger)
	{
		_logger = logger.ThrowIfNull(nameof(logger));
		_ollama = new OllamaApiClient()
			.ThrowIfNull(nameof(_ollama));
	}

	/// <summary>
	/// Initiate a chat with the large language model.
	/// </summary>
	/// <param name="messages">A collection of previous messages.</param>
	/// <param name="tools">Tools to expose to the language model. An empty array is acceptable.</param>
	/// <param name="token">Token to cancel downstream operations.</param>
	/// <returns>The new conversation history with the latest resonse as the latest response.</returns>
	public async Task<List<Message>> ChatAsync(List<Message> messages, List<Tool> tools, CancellationToken token)
	{
		var modelName = "llama3.2-vision:11b";

		_logger.LogDebug("[{ClassName}][{MethodName}] Initiating chat with message '{MessageContent}', {MessageCount} message(s) total, with model '{ModelName}' with {ToolsCount} tool(s).", nameof(OllamaLargeLanguageModel), nameof(ChatAsync), messages.Last().Content, messages.Count, modelName, tools.Count);
		await _ollama
			.Models
			.PullModelAsync(modelName)
			.EnsureSuccessAsync();

		var chatResponse = await _ollama
			.Chat
			.GenerateChatCompletionAsync(new GenerateChatCompletionRequest
			{
				Model = modelName,
				Messages = messages,
				Tools = tools
			}, token);

		return messages
			.Concat(new List<Message> { chatResponse.Message })
			.ToList();
	}

	/// <summary>
	/// Embed text to a vector collection with an embedding model.
	/// </summary>
	/// <param name="text"></param>
	/// <param name="token">Token to cancel downstream operations.</param>
	/// <returns>Returns the collection of embedding vectors.</returns>
	public async Task<List<double>> EmbedAsync(string text, CancellationToken token)
	{
		var modelName = "nomic-embed-text";

		_logger.LogDebug("[{ClassName}][{MethodName}] Embedding text '{MessageContent}' with model '{ModelName}'.", nameof(OllamaLargeLanguageModel), nameof(EmbedAsync), text, modelName);
		await _ollama.Models.PullModelAsync("nomic-embed-text").EnsureSuccessAsync();

		var embedding = await _ollama.Embeddings.GenerateEmbeddingAsync(
			model: modelName,
			prompt: text);

		return embedding
			.Embedding
			.ToList();
	}

	/// <summary>
	/// Manual cleanup of resources.
	/// </summary>
	public void Dispose()
	{
		_logger.LogDebug("[{ClassName}][{MethodName}] Disposing. unmanaged resource(s).", nameof(OllamaLargeLanguageModel), nameof(Dispose));
		_ollama.Dispose();
	}
}
