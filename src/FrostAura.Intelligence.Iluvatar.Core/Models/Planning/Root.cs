using System.Diagnostics;
using System.Xml.Serialization;

namespace FrostAura.Intelligence.Iluvatar.Core.Models.Planning
{
    /// <summary>
    /// The overarching planner wrapper.
    /// </summary>
    [XmlRoot("root")]
    [DebuggerDisplay("{Plan}")]
    public class Root
    {
        /// <summary>
        /// A plan to solve for the goal. A nested plan, if applicable.
        /// </summary>
        [XmlElement("plan")]
        public Plan Plan { get; set; }
        /// <summary>
        /// Suggested skills that could make solving problems like the goal at hand, easier in the future.
        /// </summary>
        [XmlElement("suggested-skills")]
        public SuggestedSkills SuggestedSkills { get; set; }
    }
}
