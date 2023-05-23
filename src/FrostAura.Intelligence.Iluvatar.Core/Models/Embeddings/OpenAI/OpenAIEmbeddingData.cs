namespace FrostAura.Intelligence.Iluvatar.Core.Models.Embeddings.OpenAI
{
    /// <summary>
    /// Represents a data line in the OpenAI embedding model responses.
    /// </summary>
    public class OpenAIEmbeddingData
    {
        /// <summary>
        /// A collection of embeddings. Thus this is the embedding vector.
        /// </summary>
        public List<double> Embedding { get; set; }
    }
}
