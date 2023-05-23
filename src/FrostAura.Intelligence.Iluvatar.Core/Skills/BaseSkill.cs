using FrostAura.Intelligence.Iluvatar.Core.Consts.System;
using FrostAura.Intelligence.Iluvatar.Core.Interfaces;
using FrostAura.Intelligence.Iluvatar.Core.Models.System;
using Microsoft.Extensions.Logging;

namespace FrostAura.Intelligence.Iluvatar.Core.Skills
{
    /// <summary>
    /// The root of all skills.
    /// </summary>
    public abstract class BaseSkill : ISkill
    {
        /// <summary>
        /// Describe the function of the skill in natural language.
        /// </summary>
        public abstract string Function { get; }
        /// <summary>
        /// Describe the expected input in natural language.
        /// </summary>
        public abstract string InputDescription { get; }
        /// <summary>
        /// Instance logger.
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        protected BaseSkill(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Execute the symantic component.
        /// This is the external facing call which allows for decorating the internal one.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="context">The chain execution context.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The output string.</returns>
        public async Task<string> RunAsync(string input, Dictionary<string, string> context, CancellationToken token = default)
        {
            _logger.LogInformation($"Thought: Execute the skill {this.GetType().Name} with the input '{input.Substring(0, Math.Min(input.Length, 50))}'");
            _logger.LogDebug($"Thought: Execute the skill {this.GetType().Name} with the input '{input}'.");
            Logging.CurrentIndentation += 1;
            Logging.Logs.Value = Logging
                .Logs
                .Value
                .Concat(new List<IndentedLog>
                {
                    new IndentedLog
                    {
                        Indentation = Logging.CurrentIndentation,
                        Text = $"Thought: Execute the skill {this.GetType().Name} with the input '{input.Substring(0, Math.Min(input.Length, 50))}...'"
                    }
                })
                .ToList();

            var response = await ExecuteAsync(input, context, token);

            _logger.LogInformation($"Observation: {response.Substring(0, Math.Min(response.Length, 50))}.");
            _logger.LogDebug($"Observation: {response}.");
            Logging.Logs.Value = Logging
                .Logs
                .Value
                .Concat(new List<IndentedLog>
                {
                    new IndentedLog
                    {
                        Indentation = Logging.CurrentIndentation,
                        Text = $"Observation: {response.Substring(0, Math.Min(response.Length, 50))}..."
                    }
                })
                .ToList();
            Logging.CurrentIndentation -= 1;

            return response;
        }

        /// <summary>
        /// Execute the symantic component.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="context">The chain execution context.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The output string.</returns>
        public abstract Task<string> ExecuteAsync(string input, Dictionary<string, string> context, CancellationToken token = default);
    }
}
