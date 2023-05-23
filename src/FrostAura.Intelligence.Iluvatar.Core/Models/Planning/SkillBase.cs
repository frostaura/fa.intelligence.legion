using System.Diagnostics;
using System.Xml.Serialization;

namespace FrostAura.Intelligence.Iluvatar.Core.Models.Planning
{
    /// <summary>
    /// A step / skill representation.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class SkillBase
    {
        /// <summary>
        /// The name of the skill.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }
        /// <summary>
        /// The input string for the skill. This can contain context variables.
        /// </summary>
        [XmlAttribute("input")]
        public string Input { get; set; }
    }
}
