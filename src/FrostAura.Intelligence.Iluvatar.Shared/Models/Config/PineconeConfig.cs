using System.Diagnostics;

namespace FrostAura.Intelligence.Iluvatar.Shared.Models.Config
{
    /// <summary>
    /// The OpenAI API configuration model.
    /// </summary>
    [DebuggerDisplay("{ApiEndpoint}")]
	public class OpenAIConfig
	{
		/// <summary>
		/// The API endpoint to use as the root.
		/// </summary>
		public string ApiEndpoint { get; set; }
        /// <summary>
        /// The API key to use to auth.
        /// </summary>
        public string ApiKey { get; set; }
        /// <summary>
        /// The model name of the completion model to use.
        /// </summary>
        public string ModelName { get; set; }
        /// <summary>
        /// The model name of the embedding model to use.
        /// </summary>
        public string EmbeddingModelName { get; set; }
    }
}
