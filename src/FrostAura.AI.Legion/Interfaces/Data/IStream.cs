namespace FrostAura.AI.Legion.Interfaces.Data;

/// <summary>
/// An interface for a stream that represents the comms pipeline.
/// </summary>
public interface IStream<TRequest>
{
	/// <summary>
	/// Subscribe to the stream to receive messages as they are pushed.
	/// </summary>
	/// <param name="resolver">The delegate to be run upon receiving a new message on the stream.</param>
	void Subscribe(Func<TRequest, CancellationToken, Task> resolver);
	/// <summary>
	/// A method to initialize a request on the stream.
	/// </summary>
	/// <param name="request">The request for the query.</param>
	/// <param name="token">Token to cancel downstream operations.</param>
	/// <returns>The response from the stream. Typically as resolved by other subscribed entities.</returns>
	Task<TRequest> PostAsync(TRequest request, CancellationToken token);
}
