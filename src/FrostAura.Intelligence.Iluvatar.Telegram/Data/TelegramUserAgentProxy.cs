using FrostAura.Libraries.Semantic.Core.Enums.Logging;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Models.Logging;

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

    /// <summary>
    /// A handler for when a system event occurs.
    /// </summary>
    /// <param name="scopes">A collection of all the scopes in the event system.</param>
    /// <param name="currentEvent">The current occurance / most recent event.</param>
    /// <returns>Void</returns>
    public Task OnEventAsync(List<LogItem> scopes, LogItem currentEvent)
    {
        if (!scopes.Any()) return Task.CompletedTask;

        var formattedLog = scopes
            .Where(s => s.Scope == default)
            .Select(s => GetEventTreeLogRecursively(scopes, s))
            .Aggregate((l, r) => $"{l}{Environment.NewLine}{r}");

        Console.WriteLine(formattedLog);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Recursively process events and draw a hierarchy based on their parent ids.
    /// </summary>
    /// <param name="scopes">A collection of all the scopes in the event system.</param>
    /// <param name="item">The current event.</param>
    /// <param name="index">Depth in the hierarchy.</param>
    /// <returns>The string representation.</returns>
    private string GetEventTreeLogRecursively(List<LogItem> scopes, LogItem item, int index = 1)
    {
        var delimiter = "+";
        var response = item.Type == LogType.ScopeRoot ? Enumerable
                .Range(1, index)
                .Select(i => delimiter)
                .Aggregate((l, r) => $"{l}{r}") + $" {item.Message}" + Environment.NewLine : string.Empty;

        foreach (var childItem in item.Logs)
        {
            response += Enumerable
                .Range(1, index + 1)
                .Select(i => delimiter)
                .Aggregate((l, r) => $"{l}{r}") + $" {childItem.Message}" + Environment.NewLine;

            // Write all direct children.
            foreach (var nestedChildItem in childItem.Logs)
            {
                response += GetEventTreeLogRecursively(scopes, nestedChildItem, index + 1) + Environment.NewLine;
            }
        }

        // Write all dependent items.
        var dependents = scopes
            .Where(s => s.Scope?.Id == item.Id);

        foreach (var dependentItem in dependents)
        {
            response += GetEventTreeLogRecursively(scopes, dependentItem, index + 1) + Environment.NewLine;
        }

        return response;
    }
}
