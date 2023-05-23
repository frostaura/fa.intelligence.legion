namespace FrostAura.Intelligence.Iluvatar.Core.Attributes.Skills
{
    /// <summary>
    /// An attribute for capturing a context variable that a skill requires.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SkillRequiredContextVariableAttribute : Attribute
    {
        /// <summary>
        /// The name of the skill's required context variable's name.
        /// </summary>
        public string VariableName { get; private set; }
        /// <summary>
        /// The description of the skill's required context variable's description.
        /// </summary>
        public string VariableDescription { get; private set; }

        /// <summary>
        /// Overloaded constructor to allow for passing parameters.
        /// </summary>
        /// <param name="variableDescription">The description for the skill's required context variable's description.</param>
        public SkillRequiredContextVariableAttribute(string variableName, string variableDescription)
        {
            VariableName = variableName;
            VariableDescription = variableDescription;
        }
    }
}
