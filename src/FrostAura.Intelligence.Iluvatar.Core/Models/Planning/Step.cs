using FrostAura.Intelligence.Iluvatar.Core.Consts.Cognitive;
using FrostAura.Intelligence.Iluvatar.Core.Extensions.Cognitive;
using FrostAura.Intelligence.Iluvatar.Core.Skills;
using FrostAura.Intelligence.Iluvatar.Core.Skills.Cognitive;
using FrostAura.Intelligence.Iluvatar.Core.Skills.Core;
using FrostAura.Libraries.Core.Extensions.Validation;
using System.Xml.Serialization;

namespace FrostAura.Intelligence.Iluvatar.Core.Models.Planning
{
    /// <summary>
    /// The shared foundation for skill attributes.
    /// </summary>
    public class Step : SkillBase
    {
        /// <summary>
        /// The context variable to set with the value of the output of this skill, if any.
        /// </summary>
        [XmlAttribute("set-context-variable")]
        public string OutputContextVariableKey { get; set; }
        /// <summary>
        /// The reasoning behind the approach.
        /// </summary>
        [XmlAttribute("reasoning")]
        public string Reasoning { get; set; }
        /// <summary>
        /// Self-critisism on the approach.
        /// </summary>
        [XmlAttribute("critisism")]
        public string Critisism { get; set; }
        /// <summary>
        /// Collection of context arguments to set for the step.
        /// </summary>
        [XmlElement("arguments")]
        public Arguments Arguments { get; set; }

        /// <summary>
        /// Execute the step's actual skill.
        /// </summary>
        /// <param name="context">The chain execution context.</param>
        /// <param name="planner">The original planner skill to reuse for execution.</param>
        /// <param name="llmSkill">Allows for passing {PromptVariables.INPUT} to an OpenAI large language model and returning the model's reponse.</param>
        /// <param name="availableSkills">All available skills.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>Void</returns>
        public async Task<string> ExecuteStepAsync(Dictionary<string, string> context, BaseSkill planner, BaseSkill llmSkill, IEnumerable<BaseSkill> availableSkills, CancellationToken token)
        {
            var isPlannerStep = Name == typeof(PlannerSkill).FullName;

            if (isPlannerStep) throw new NotImplementedException("Implement if the step is a planner for recursive resolution.");

            var skill = availableSkills
                .SingleOrDefault(avsk => avsk.GetType().FullName == Name);

            if(skill == default)
            {
                skill.ThrowIfNull($"Could not resolve the required skill for the step named '{Name}'.");
            }
            var interpolatedInput = Input;

            foreach (var variable in context)
            {
                interpolatedInput = interpolatedInput
                    .Replace($"${variable.Key}", variable.Value)
                    .Replace(variable.Key, variable.Value);
            }

            // TODO: Infer the hierarchy from this. (If input exists, move it to something like input.X and we can keep track that way.
            Arguments.ContextArguments.ForEach(a => context[a.Name] = a.Value.Contextualize(context));
            context[PromptVariables.INPUT] = interpolatedInput;

            var result = await skill.RunAsync(interpolatedInput, context, token);

            if (!string.IsNullOrWhiteSpace(OutputContextVariableKey))
            {
                context[OutputContextVariableKey] = result;
            }

            return result;
        }
    }
}
