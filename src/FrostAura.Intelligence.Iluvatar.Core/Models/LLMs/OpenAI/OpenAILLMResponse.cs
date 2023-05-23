using System.Diagnostics;

namespace FrostAura.Intelligence.Iluvatar.Core.Models.LLMs.OpenAI
{
    /// <summary>
    /// The root response object from the OpenAI API.
    /// </summary>
    [DebuggerDisplay("{Model}")]
    public class OpenAILLMResponse
    {
        /// <summary>
        /// The name of the model used.
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// Response choices.
        /// </summary>
        public List<OpenAILLMChoice> Choices { get; set; }
    }
}
