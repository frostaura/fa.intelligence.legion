using FrostAura.Intelligence.Iluvatar.Core.Consts.Cognitive;
using Microsoft.Extensions.Logging;

namespace FrostAura.Intelligence.Iluvatar.Core.Skills.Core
{
    /// <summary>
    /// Take in a {PromptVariables.INPUT} that specifies how long to wait for in milliseconds before proceeding. This is great for introducing an artificial delay in a call chain.
    /// </summary>
    public class WaitSkill : BaseSkill
    {
        /// <summary>
        /// The description of the expected input.
        /// </summary>
        public override string InputDescription => "The integer value in milliseconds for the delay. For example, for one second: 1000";
        /// <summary>
        /// The function of the skill.
        /// </summary>
        public override string Function => $"Take in a {PromptVariables.INPUT} that specifies how long to wait for in milliseconds before proceeding. This is great for introducing an artificial delay in a call chain.";

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        public WaitSkill(ILogger<WaitSkill> logger)
            : base(logger)
        { }

        /// <summary>
        /// Take in an integer value, $INPUT, that specifies how long to wait for in milliseconds before proceeding. This is great for introducing a artificial delay in a call chain.
        /// </summary>
        /// <param name="input">The stringified milliseconds integer value to wait for.</param>
        /// <param name="context">The chain execution context.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The output string.</returns>
        public override async Task<string> ExecuteAsync(string input, Dictionary<string, string> context, CancellationToken token = default)
        {
            var millisecondsToWait = int.Parse(input);

            await Task.Delay(TimeSpan.FromMilliseconds(millisecondsToWait));

            return $"Successfully finished waiting for {millisecondsToWait} milliseconds.";
        }
    }
}
