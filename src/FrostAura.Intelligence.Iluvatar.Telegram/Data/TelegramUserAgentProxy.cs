using FrostAura.Libraries.Semantic.Core.Interfaces.Data;

namespace FrostAura.Intelligence.Iluvatar.Telegram.Data;

/// <summary>
/// Telegram user proxy.
/// </summary>
public class TelegramUserAgentProxy : IUserProxyDataAccess
{
    /// <summary>
    /// For now, a default response to user questions.
    /// </summary>
    /// <param name="question">The question that the machine has for the human.</param>
    /// <param name="token">Token for cancelling downstream operations.</param>
    /// <returns>The answer.</returns>
    public Task<string> AskUserAsync(string question, CancellationToken token)
    {
        // TODO: Implement Telegram feedback loop.
        return Task.FromResult("The user is unavailable to answer and has left you with full authority to decide on things.");
    }
}
