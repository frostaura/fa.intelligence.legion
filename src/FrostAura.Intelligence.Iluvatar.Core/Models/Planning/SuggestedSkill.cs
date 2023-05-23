using System.Diagnostics;
using System.Xml.Serialization;

namespace FrostAura.Intelligence.Iluvatar.Core.Models.Planning
{
    /// <summary>
    /// A suggested skill from the planner.
    /// </summary>
    [DebuggerDisplay("{Name} - {SuggestedApproach}")]
    public class SuggestedSkill : SkillBase
    {
        /// <summary>
        /// The suggested approach for implementing the skill.
        /// </summary>
        [XmlAttribute("how-suggestion")]
        public string SuggestedApproach { get; set; }
    }
}
