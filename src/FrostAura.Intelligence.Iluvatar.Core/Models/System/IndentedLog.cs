namespace FrostAura.Intelligence.Iluvatar.Core.Models.System
{
    /// <summary>
    /// An indented log line.
    /// </summary>
    public class IndentedLog
    {
        /// <summary>
        /// The indentation of the log line.
        /// </summary>
        public int Indentation { get; set; }
        /// <summary>
        /// The log's body.
        /// </summary>
        public string Text { get; set; }
    }
}
