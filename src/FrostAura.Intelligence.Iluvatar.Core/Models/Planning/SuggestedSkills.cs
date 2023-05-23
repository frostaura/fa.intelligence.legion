using System.Xml.Serialization;

namespace FrostAura.Intelligence.Iluvatar.Core.Models.Planning
{
    /// <summary>
    /// Suggested skills that could make solving problems like the goal at hand, easier in the future.
    /// </summary>
    public class SuggestedSkills
    {
        /// <summary>
        /// Skills to execute / steps.
        /// </summary>
        [XmlElement("skill")]
        public List<SuggestedSkill> Skills { get; set; }
    }
}
