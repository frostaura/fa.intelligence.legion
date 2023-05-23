namespace FrostAura.Intelligence.Iluvatar.Core.Extensions.String
{
    /// <summary>
    /// String splitting extensions.
    /// </summary>
    public static class Splitting
    {
        /// <summary>
        /// Split a text blob into smaller chunks.
        /// </summary>
        /// <param name="text">The text blob to split.</param>
        /// <param name="size">The max characters in each chunk before overlap.</param>
        /// <param name="overlap">The character count for subsequent chunks should include from the previous.</param>
        /// <returns>Text chunks.</returns>
        public static List<string> ToChunks(this string text, int size, int overlap)
        {
            List<string> chunks = new List<string>();
            int startIndex = 0;

            while (startIndex < text.Length)
            {
                int endIndex = Math.Min(startIndex + size, text.Length);
                chunks.Add(text.Substring(startIndex, endIndex - startIndex));

                startIndex += (size - overlap);
            }

            return chunks;
        }
    }
}
