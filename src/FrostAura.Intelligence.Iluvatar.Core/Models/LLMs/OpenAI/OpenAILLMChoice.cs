using System.Diagnostics;

namespace FrostAura.Intelligence.Iluvatar.Core.Models.LLMs.OpenAI
{
    /// <summary>
    /// A response choice from the OpenAI API.
    /// </summary>
    [DebuggerDisplay("{Message}")]
    public class OpenAILLMChoice
    {
        /// <summary>
        /// The response message.
        /// </summary>
        public OpenAILLMMessage Message { get; set; }
    }
}
