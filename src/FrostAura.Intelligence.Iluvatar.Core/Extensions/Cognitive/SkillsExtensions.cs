using FrostAura.Intelligence.Iluvatar.Core.Attributes.Skills;
using FrostAura.Intelligence.Iluvatar.Core.Skills;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace FrostAura.Intelligence.Iluvatar.Core.Extensions.Cognitive
{
    /// <summary>
    /// A collection of skills extensions.
    /// </summary>
    public static class SkillsExtensions
    {
        /// <summary>
        /// Get a collection of all available skill types.
        /// </summary>
        /// <param name="assembly">The skills assembly.</param>
        /// <returns>A collection of all available skill types.</returns>
        public static IEnumerable<Type> GetAllSkillTypesInAssembly(this Assembly assembly)
        {
            var availableSkillTypesInAssembly = assembly
                .GetTypes()
                .Where(t => t.IsAssignableTo(typeof(BaseSkill)) & !t.IsAbstract);

            return availableSkillTypesInAssembly
                .WithAllCoreSkillTypes();
        }

        /// <summary>
        /// Get a collection of all core skill types and union it onto the passed in collection.
        /// </summary>
		/// <param name="availableSkillTypes">Available skill types to union the core skills onto.</param>
        /// <returns>A collection of all core skill types.</returns>
        public static IEnumerable<Type> WithAllCoreSkillTypes(this IEnumerable<Type> availableSkillTypes)
        {
            var assembly = typeof(SkillsExtensions).Assembly;
            var availableSkillTypesInAssembly = assembly
                .GetTypes()
                .Where(t => t.IsAssignableTo(typeof(BaseSkill)) & !t.IsAbstract);

            return availableSkillTypes
                .Concat(availableSkillTypesInAssembly)
                .Distinct();
        }

        /// <summary>
        /// Convert a skill to a string representation that can be useful in prompts.
        /// </summary>
        /// <param name="skill">The skill instance.</param>
        /// <returns>The injectable string for prompts.</returns>
        public static string ToPromptString(this BaseSkill skill)
        {
            var contextVariablesRequired = skill
                .GetType()
                .GetMember(nameof(BaseSkill.ExecuteAsync))
                .FirstOrDefault()?
                .GetCustomAttributes<SkillRequiredContextVariableAttribute>()
                .Select(m => $"{m.VariableName}=\"{m.VariableDescription}\"");

            if (!contextVariablesRequired.Any())
            {
                return $"<skill name=\"{skill.GetType().FullName}\" function=\"{skill.Function}\" input=\"{skill.InputDescription}\" />";

            }
            else
            {
                var contextVariablesAttributeStr = contextVariablesRequired
                    .Aggregate((l, r) => $"{l} {r}");
                var baseSkillNode = $"<skill name=\"{skill.GetType().FullName}\" function=\"{skill.Function}\" input=\"{skill.InputDescription}\" {contextVariablesAttributeStr} />";

                return baseSkillNode;
            }
        }

        /// <summary>
        /// Filter a collection of skills by specific type.
        /// </summary>
        /// <param name="skills">The skills to filter.</param>
        /// <param name="typesToInclude">The types of skills to include.</param>
        /// <returns>The instances of the select types to include.</returns>
        public static IEnumerable<BaseSkill> OnlyInclude(this IEnumerable<BaseSkill> skills, params Type[] typesToInclude)
        {
            var response = skills
                .Where(s => typesToInclude.Contains(s.GetType()))
                .ToList();

            return response;
        }

        /// <summary>
        /// Comvert a collection of skills to string representations that can be useful in prompts.
        /// </summary>
        /// <param name="skills">The skills to convert.</param>
        /// <returns>The injectable strings for prompts.</returns>
        public static IEnumerable<string> ToPromptStrings(this IEnumerable<BaseSkill> skills)
        {
            return skills
                .Select(s => s.ToPromptString());
        }
    }
}
