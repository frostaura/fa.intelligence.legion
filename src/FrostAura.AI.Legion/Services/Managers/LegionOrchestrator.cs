﻿using System.Text.Json;
using FrostAura.AI.Legion.Enums.Communication;
using FrostAura.AI.Legion.Extensions.Models;
using FrostAura.AI.Legion.Interfaces.Data;
using FrostAura.AI.Legion.Interfaces.Managers;
using FrostAura.AI.Legion.Models.Common;
using FrostAura.AI.Legion.Models.Communication;
using FrostAura.Libraries.Core.Extensions.Validation;
using Microsoft.Extensions.Logging;
using Ollama;

/// <summary>
/// The main entry point to the Legion system.
/// </summary>
public class LegionOrchestrator : ISemanticOrchestrator
{
	/// <summary>
	/// The conversational stream.
	/// </summary>
	private readonly IStream<StreamMessage> _stream;
	/// <summary>
	/// The large language model instance.
	/// </summary>
	private readonly ILargeLanguageModel _largeLanguageModel;
	/// <summary>
	/// Instance logger.
	/// </summary>
	private readonly ILogger _logger;

	/// <summary>
	/// Overloaded constructor for injecting dependencies.
	/// </summary>
	/// <param name="stream">The conversational stream.</param>
	/// <param name="largeLanguageModel">The large language model instance.</param>
	public LegionOrchestrator(IStream<StreamMessage> stream, ILargeLanguageModel largeLanguageModel, ILogger<LegionOrchestrator> logger)
	{
		_logger = logger.ThrowIfNull(nameof(logger));
		_stream = stream.ThrowIfNull(nameof(stream));
		_stream.Subscribe(HandleMessageAsync);
		_largeLanguageModel = largeLanguageModel.ThrowIfNull(nameof(largeLanguageModel));
	}

	/// <summary>
	/// Initiate a workload with the Legion system.
	/// </summary>
	/// <param name="request">The request body for the workload.</param>
	/// <param name="token">Token to cancel downstream operations.</param>
	/// <returns>The response from the Legion system.</returns>
	public async Task<LegionResponse> ChatAsync(LegionRequest request, CancellationToken token)
	{
		_logger.LogDebug("[{ClassName}][{MethodName}] Initiating a chat with message '{MessageContent}'.", nameof(LegionOrchestrator), nameof(ChatAsync), request.Content.Last().Content);

		var responeMessage = await _stream.PostAsync(new StreamMessage
		{
			Request = request
		}, token);

		return responeMessage.Response;
	}

	/// <summary>
	/// Initiate a workload with the Legion system.
	/// </summary>
	/// <param name="request">The text request body for the workload.</param>
	/// <param name="token">Token to cancel downstream operations.</param>
	/// <returns>The text response from the Legion system.</returns>
	public async Task<string> ChatAsync(string request, CancellationToken token)
	{
		_logger.LogDebug("[{ClassName}][{MethodName}] Initiating a simplified chat with message '{MessageContent}'.", nameof(LegionOrchestrator), nameof(ChatAsync), request);

		var response = await ChatAsync(request.ToLegionRequest(), token);

		return response
			.Content
			.First()
			.Content;
	}

	/// <summary>
	/// Handle new messages.
	/// </summary>
	/// <param name="message">The message to process.</param>
	/// <param name="token">Token to cancel downstream operations.</param>
	private async Task HandleMessageAsync(StreamMessage message, CancellationToken token)
	{
		if (message.Type == MessageDirection.Response) return;

		_logger.LogDebug("[{ClassName}][{MethodName}] Processing new incoming request message '{MessageContent}'.", nameof(LegionOrchestrator), nameof(HandleMessageAsync), message.Request.Content.Last().Content);

		var tools = new List<CSharpToJsonSchema.Tool>
		{
			new CSharpToJsonSchema.Tool
			{
				Name = "get_current_time",
				Description = "Get the current time in the user's locale.",
				Parameters = JsonSerializer.Serialize(new
				{
					type = "object",
					properties = new
					{
						days_of_forecast = new
						{
							type = "integer",
							description = "The number of days to forecast."
						}
					},
					required = new[] { "days_of_forecast" }
				})
			}
		}
		.AsOllamaTools()
		.ToList();
		var response = await _largeLanguageModel.ChatAsync(message.Request.Content, tools, token);

		// TODO: This is where we should register the selector / caption agent.
		await _stream.PostAsync(new StreamMessage
		{
			Type = MessageDirection.Response,
			ConversationId = message.ConversationId,
			Response = new LegionResponse
			{
				Content = new List<Message>
				{
					new Message
					{
						Role = MessageRole.Assistant,
						Content = response.Last().Content
					}
				}
			}
		}, token);
	}
}
