using System.Diagnostics;
using System.Xml.Serialization;

namespace FrostAura.Intelligence.Iluvatar.Core.Models.Planning
{
    /// <summary>
    /// An argument that should be set as a context variable to a step.
    /// </summary>
    [DebuggerDisplay("{Name} - {Value}")]
    public class Argument
    {
        /// <summary>
        /// Name of the variable to set.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }
        /// <summary>
        /// Value of the variable to set. This could be a variable.
        /// </summary>
        [XmlAttribute("value")]
        public string Value { get; set; }
    }
}
