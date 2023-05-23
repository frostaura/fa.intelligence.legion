using System.Diagnostics;
using System.Xml.Serialization;

namespace FrostAura.Intelligence.Iluvatar.Core.Models.Planning
{
    /// <summary>
    /// The arguments that should be set as a context variables to a step.
    /// </summary>
    [DebuggerDisplay("{ContextArguments}")]
    public class Arguments
    {
        /// <summary>
        /// The suggested approach for implementing the skill.
        /// </summary>
        [XmlElement("argument")]
        public List<Argument> ContextArguments { get; set; }
    }
}
