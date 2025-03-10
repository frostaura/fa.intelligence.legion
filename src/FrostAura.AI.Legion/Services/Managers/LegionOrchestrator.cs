using FrostAura.AI.Legion.Enums.Communication;
using FrostAura.AI.Legion.Extensions.Models;
using FrostAura.AI.Legion.Interfaces.Data;
using FrostAura.AI.Legion.Interfaces.Managers;
using FrostAura.AI.Legion.Models.Common;
using FrostAura.AI.Legion.Models.Communication;
using FrostAura.Libraries.Core.Extensions.Validation;

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
	/// Overloaded constructor for injecting dependencies.
	/// </summary>
	/// <param name="stream">The conversational stream.</param>
	public LegionOrchestrator(IStream<StreamMessage> stream)
	{
		_stream = stream.ThrowIfNull(nameof(stream));
		_stream.Subscribe(HandleMessageAsync);
	}

	/// <summary>
	/// Initiate a workload with the Legion system.
	/// </summary>
	/// <param name="request">The request body for the workload.</param>
	/// <param name="token">Token to cancel downstream operations.</param>
	/// <returns>The response from the Legion system.</returns>
	public async Task<LegionResponse> ChatAsync(LegionRequest request, CancellationToken token)
	{
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

		Console.WriteLine($"Message received.");
		// TODO: This is where we should register the selector / caption agent.
		await _stream.PostAsync(new StreamMessage
		{
			Type = MessageDirection.Response,
			ConversationId = message.ConversationId,
			Response = new LegionResponse
			{
				Content = new List<MessageContent>
				{
					new MessageContent
					{
						ContentType = ContentType.Text,
						Content = "Exmple response"
					}
				}
			}
		}, token);
	}
}
