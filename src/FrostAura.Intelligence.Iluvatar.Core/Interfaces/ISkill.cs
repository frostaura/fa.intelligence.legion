using System.ComponentModel;

namespace FrostAura.Intelligence.Iluvatar.Core.Interfaces
{
    /// <summary>
    /// An interface for a skill the semantic engine can use.
    /// </summary>
    public interface ISkill : ISemanticExecutable
    {
        /// <summary>
        /// The function of the skill.
        /// </summary>
        [Description("The function of the skill.")]
        string Function { get; }
    }
}
