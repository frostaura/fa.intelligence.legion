using System.Diagnostics;

namespace FrostAura.Intelligence.Iluvatar.Core.Models.Memory
{
    /// <summary>
    /// Model of a semantic memory.
    /// </summary>
    [DebuggerDisplay("{Metadata}")]
    public class Memory
    {
        /// <summary>
        /// Details about where the memory came from.
        /// </summary>
        public MemoryMetadata Metadata { get; set; }
        /// <summary>
        /// The embedding vector for the memory.
        /// </summary>
        public List<double> Embeddings { get; set; }
    }
}
