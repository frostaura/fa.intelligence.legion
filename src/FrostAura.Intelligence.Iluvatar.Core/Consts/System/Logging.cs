using FrostAura.Intelligence.Iluvatar.Core.Models.System;
using FrostAura.Libraries.Core.Extensions.Reactive;
using FrostAura.Libraries.Core.Interfaces.Reactive;

namespace FrostAura.Intelligence.Iluvatar.Core.Consts.System
{
    /// <summary>
    /// Logging consts.
    /// </summary>
    public class Logging
    {
        /// <summary>
        /// The current zero-based indentation of the logs. 0 being root-level.
        /// </summary>
        public static int CurrentIndentation = 0;
        /// <summary>
        /// Observable logs container.
        /// </summary>
        public static IObservedValue<List<IndentedLog>> Logs { get; private set; } = new List<IndentedLog>().AsObservedValue();
    }
}
