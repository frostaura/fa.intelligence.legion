using FrostAura.AI.Legion.Interfaces.Data;
using FrostAura.AI.Legion.Models.Communication;
using FrostAura.Libraries.Core.Extensions.Validation;
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
	/// Overloaded constructor for injecting dependencies.
	/// </summary>
	public OllamaLargeLanguageModel()
	{
		_ollama = new OllamaApiClient().ThrowIfNull(nameof(_ollama));
	}

	/// <summary>
	/// Initiate a chat with the large language model.
	/// </summary>
	/// <param name="conversationHistory">A collection of previous messages.</param>
	/// <param name="token">Token to cancel downstream operations.</param>
	/// <returns>The new conversation history with the latest resonse as the latest response.</returns>
	public async Task<List<MessageContent>> ChatAsync(List<MessageContent> conversationHistory, CancellationToken token)
	{
		var models = await _ollama.Models.ListModelsAsync();

		await _ollama.Models.PullModelAsync("nomic-embed-text").EnsureSuccessAsync();

		// Generating an embedding
		var embedding = await _ollama.Embeddings.GenerateEmbeddingAsync(
			model: "nomic-embed-text",
			prompt: "hello");

		// Streaming a completion directly into the console
		// keep reusing the context to keep the chat topic going
		await _ollama.Models.PullModelAsync("llama3.2").EnsureSuccessAsync();
		IList<long>? context = null;
		var enumerable = _ollama.Completions.GenerateCompletionAsync("llama3.2", "answer 5 random words");
		await foreach (var response in enumerable)
		{
			Console.WriteLine($"> {response.Response}");

			context = response.Context;
		}

		var lastResponse = await _ollama.Completions.GenerateCompletionAsync("llama3.2", "answer 123", stream: false, context: context).WaitAsync();

		throw new NotImplementedException("TODO: Implement ChatAsync");
	}

	/// <summary>
	/// Manual cleanup of resources.
	/// </summary>
	public void Dispose()
	{
		_ollama.Dispose();
	}
}
