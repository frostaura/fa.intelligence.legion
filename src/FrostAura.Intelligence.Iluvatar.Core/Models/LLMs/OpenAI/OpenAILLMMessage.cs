using System.Diagnostics;

namespace FrostAura.Intelligence.Iluvatar.Core.Models.LLMs.OpenAI
{
    /// <summary>
    /// The message payload for an OpenAI model.
    /// </summary>
    [DebuggerDisplay("{Role} => {Content}")]
    public class OpenAILLMMessage
    {
        /// <summary>
        /// The role of the message creator.
        /// - assistant => AI responses.
        /// - system => Instructional messages.
        /// - user => User messages.
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// The string content of the message.
        /// </summary>
        public string Content { get; set; }
    }
}
