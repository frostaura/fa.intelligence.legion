using System.Diagnostics;
using System.Xml.Serialization;

namespace FrostAura.Intelligence.Iluvatar.Core.Models.Planning
{
    /// <summary>
    /// A plan to solve for the goal. A nested plan, if applicable.
    /// </summary>
    [DebuggerDisplay("{Goal}")]
    public class Plan
    {
        /// <summary>
        /// The goal that the planner should solve for.
        /// </summary>
        public string Goal { get; set; }
        /// <summary>
        /// The plan's XML body.
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// The reasoning for the plan's strategy.
        /// </summary>
        [XmlAttribute("reasoning")]
        public string Reasoning { get; set; }
        /// <summary>
        /// Self-critisism on the strategy.
        /// </summary>
        [XmlAttribute("critisism")]
        public string Critisism { get; set; }
        /// <summary>
        /// Skills to execute / steps.
        /// </summary>
        [XmlElement("skill")]
        public List<Step> Steps { get; set; }
        /// <summary>
        /// The output of the plan's final step, if set.
        /// </summary>
        [XmlIgnore]
        public string? Output { get; set; }
    }
}
