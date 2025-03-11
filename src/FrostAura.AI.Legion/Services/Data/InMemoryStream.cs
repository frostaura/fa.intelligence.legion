using FrostAura.AI.Legion.Enums.Communication;
using FrostAura.AI.Legion.Interfaces.Data;
using FrostAura.AI.Legion.Models.Communication;
using FrostAura.Libraries.Core.Extensions.Validation;
using Microsoft.Extensions.Logging;

namespace FrostAura.AI.Legion.Services.Data;

/// <summary>
/// An in-memory, local implementation of a stream.
/// </summary>
public class InMemoryStream : IStream<StreamMessage>
{
	/// <summary>
	/// Instance logger.
	/// </summary>
	private readonly ILogger _logger;
	/// <summary>
	/// The local stream.
	/// </summary>
	private readonly List<StreamMessage> _stream = new List<StreamMessage>();
	/// <summary>
	/// An event that may be subscribed to in order to receive updates on the stream.
	/// </summary>
	private event Func<StreamMessage, CancellationToken, Task>? _streamChanged;
	/// <summary>
	/// A buffer of all pending request messages that have not been resolved yet.
	/// </summary>
	private Dictionary<Guid, TaskCompletionSource<StreamMessage>> _pendingRequestMessages = new Dictionary<Guid, TaskCompletionSource<StreamMessage>>();

	/// <summary>
	/// Overloaded constructor to allow for injecting dependencies.
	/// </summary>
	/// <param name="logger">Instance logger.</param>
	public InMemoryStream(ILogger<InMemoryStream> logger)
	{
		_logger = logger.ThrowIfNull(nameof(logger));
		_streamChanged -= ProcessResponseMessage;
		_streamChanged += ProcessResponseMessage;
	}

	/// <summary>
	/// Subscribe to the stream to receive messages as they are pushed.
	/// </summary>
	/// <param name="resolver">The delegate to be run upon receiving a new message on the stream.</param>
	public void Subscribe(Func<StreamMessage, CancellationToken, Task> resolver)
	{
		_streamChanged -= resolver;
		_streamChanged += resolver;

		_logger.LogDebug("[{ClassName}] Subscribed successfully.", nameof(InMemoryStream));
	}

	/// <summary>
	/// A method to initialize a request on the stream.
	/// </summary>
	/// <param name="request">The request for the query.</param>
	/// <param name="token">Token to cancel downstream operations.</param>
	/// <returns>The response from the stream. Typically as resolved by other subscribed entities.</returns>
	public async Task<StreamMessage> PostAsync(StreamMessage message, CancellationToken token)
	{
		_logger.LogDebug("[{ClassName}] Posting to the stream.", nameof(InMemoryStream));
		_stream.Add(message);

		if (message.Type == MessageDirection.Request)
		{
			_logger.LogDebug("[{ClassName}] Registering completion source.", nameof(InMemoryStream));
			_pendingRequestMessages[message.ConversationId] = new TaskCompletionSource<StreamMessage>();
		}

		var handlers = _streamChanged
			.GetInvocationList()
			.Cast<Func<StreamMessage, CancellationToken, Task>>();
		var tasks = handlers
			.Select(handler => handler(message, token));

		_logger.LogDebug("[{ClassName}] Triggering all event listeners.", nameof(InMemoryStream));
		await Task.WhenAll(tasks);

		if (message.Type == MessageDirection.Response) return message;

		return await _pendingRequestMessages[message.ConversationId].Task;
	}

	/// <summary>
	/// Handle new response messages and resolve the original requests.
	/// </summary>
	/// <param name="message">The message to process.</param>
	/// <param name="token">Token to cancel downstream operations.</param>
	private async Task ProcessResponseMessage(StreamMessage message, CancellationToken token)
	{
		if (message.Type != MessageDirection.Response) return;

		var requestMessage = _stream
			.Where(m => m.Type == MessageDirection.Request)
			.FirstOrDefault(m => m.ConversationId == message.ConversationId);

		if (requestMessage == default) return;
		if (!_pendingRequestMessages.ContainsKey(message.ConversationId)) return;

		_logger.LogDebug("[{ClassName}] Handling response message for conversation id {MessageConversationId}.", nameof(InMemoryStream), message.ConversationId);
		_pendingRequestMessages[message.ConversationId]
			.SetResult(message);
	}
}
