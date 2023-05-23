namespace FrostAura.Intelligence.Iluvatar.Core.Consts.Cognitive
{
    /// <summary>
    /// A collection of core prompt variable keys.
    /// </summary>
    public class PromptVariables
    {
        public const string INPUT = "INPUT";
        public const string AVAILABLE_SKILLS = "{{AVAILABLE_SKILLS}}";

        #region Memory
        public const string SOURCE = "source";
        public const string CHUNK_SIZE = "chunk-size";
        public const string OVERLAP_SIZE = "overlap-size";
        public const string MEMORY_COUNT = "memory-count";
        #endregion
    }
}
