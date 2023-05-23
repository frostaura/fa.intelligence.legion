using System.ComponentModel;

namespace FrostAura.Intelligence.Iluvatar.Core.Interfaces
{
    /// <summary>
    /// The fundamental interfact to all executable components in the semantic engine.
    /// </summary>
    public interface ISemanticExecutable
    {
        /// <summary>
        /// The description of the expected input.
        /// </summary>
        [Description("The description of the expected input.")]
        string InputDescription { get; }
        /// <summary>
        /// Execute the symantic component.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="context">The chain execution context.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The output string.</returns>
        Task<string> RunAsync(string input, Dictionary<string, string> context, CancellationToken token = default);
    }
}
