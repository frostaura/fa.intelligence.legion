using System.Diagnostics;

namespace FrostAura.Intelligence.Iluvatar.Core.Models.Memory
{
    /// <summary>
    /// Model of a semantic memory's source details.
    /// </summary>
    [DebuggerDisplay("{IndexInChunk}: {Source} - {Text}")]
    public class MemoryMetadata
    {
        /// <summary>
        /// The unique identifier for the memory chunk.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The source of the memory.
        /// E.g. http://google.com?q=test, MyFile.pdf etc.
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// The count of characters that was used to split the source text into chunks.
        /// </summary>
        public int ChunkSize { get; set; } = 1000;
        /// <summary>
        /// The count of characters to overlap and get appended to the original split of a chunk.
        /// I.e. If size = 1000 & overlap = 200 then chunk 2's start would be 800 and its end would be 2000.
        /// </summary>
        public int ChunkOverlap { get; set; } = 200;
        /// <summary>
        /// A zero-based index for which chunk number in the larger collection, this memory is for.
        /// </summary>
        public int IndexInChunk { get; set; }
        /// <summary>
        /// The text content of the chunk.
        /// </summary>
        public string Text { get; set; }
    }
}
