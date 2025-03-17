using System.Text.Json;
using System.Text.Json.Nodes;
using FrostAura.AI.Legion.Interfaces.Data;
using FrostAura.Libraries.Core.Extensions.Validation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
	public OllamaLargeLanguageModel(ILogger<OllamaLargeLanguageModel> logger, IHttpClientFactory clientFactory)
	{
		var handler = new LoggingHandler(new HttpClientHandler());
		var client = new HttpClient(handler); //clientFactory.CreateClient()

		_logger = logger.ThrowIfNull(nameof(logger));
		_ollama = new OllamaApiClient(httpClient: client)
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
		var modelName = "llama3.2";

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

		// TODO: Take progress further.

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

// TODO: Migrate this middleware elsewhere.
public class LoggingHandler : DelegatingHandler
{
	public LoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var newRequest = request;

		Console.WriteLine("Request:");
		Console.WriteLine(request);

		if (request.Content != null)
		{
			var reqBody = await request.Content.ReadAsStringAsync();
			var root = JsonNode.Parse(reqBody);
			var tools = root["tools"]?.AsArray();

			if (tools != null)
			{
				foreach (var tool in tools)
				{
					var function = tool?["function"];

					if (function != null)
					{
						// Parse the parameters string into an object
						var parametersJson = function["parameters"]?.GetValue<string>();
						var parsedParameters = JsonNode.Parse(parametersJson);
						var propertiesJson = parsedParameters["properties"]?.ToString();

						if (propertiesJson != null)
						{
							// Parse the "properties" JSON string into an object
							var parsedProperties = JsonNode.Parse(propertiesJson);

							// Replace the "properties" field in parsedParameters
							parsedParameters["properties"] = parsedProperties;
						}

						function["parameters"] = parsedParameters;
					}
				}
			}

			var newRequestJsonStr = root.ToJsonString(new JsonSerializerOptions { WriteIndented = true });

			newRequest = new HttpRequestMessage(request.Method, request.RequestUri)
			{
				Content = new StringContent(newRequestJsonStr)
			};

			foreach (var header in request.Headers)
			{
				newRequest.Headers.Add(header.Key, header.Value);
			}

			Console.WriteLine(reqBody);
		}

		var response = await base.SendAsync(newRequest, cancellationToken);

		Console.WriteLine("Response:");
		Console.WriteLine(response.ToString());
		Console.WriteLine(await response.Content.ReadAsStringAsync());

		return response;
	}
}
