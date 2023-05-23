using System.Text;
using FrostAura.Intelligence.Iluvatar.Core.Consts.Cognitive;
using FrostAura.Intelligence.Iluvatar.Core.Consts.System;
using FrostAura.Intelligence.Iluvatar.Core.Models.LLMs.OpenAI;
using FrostAura.Intelligence.Iluvatar.Core.Models.System;
using FrostAura.Intelligence.Iluvatar.Shared.Models.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace FrostAura.Intelligence.Iluvatar.Core.Skills.Cognitive
{
    /// <summary>
    /// Allows for passing {PromptVariables.INPUT} to an OpenAI large language model and returning the model's reponse.
    /// </summary>
    public class LLMSkill : BaseSkill
    {
        /// <summary>
        /// The description of the expected input.
        /// </summary>
        public override string InputDescription => "A natural language query, question or statement to call the LLM with.";
        /// <summary>
        /// The function of the skill.
        /// </summary>
        public override string Function => $"Allows for passing {PromptVariables.INPUT} to an OpenAI large language model and returning the model's reponse. This skill excels at natural language understanding, text generation, language translation, question answering, text completion, sentiment analysis, text-based games, and code generation. However, it has limitations, such as generating incorrect or nonsensical information and lacking human-like reasoning capabilities.";
        /// <summary>
        /// Retry policy for when the LLM skill fails.
        /// </summary>
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        /// <summary>
        /// OpenAI configuration.
        /// </summary>
        private readonly OpenAIConfig _openAIConfig;

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="openAIOptions">OpenAI configuration options.</param>
        /// <param name="logger">Logger instance.</param>
        public LLMSkill(IOptions<OpenAIConfig> openAIOptions, ILogger<LLMSkill> logger)
            : base(logger)
        {
            _retryPolicy = Policy
                .Handle<Exception>()
                .OrResult<HttpResponseMessage>(m => !m.IsSuccessStatusCode)
                .WaitAndRetryAsync(7, retryCount => TimeSpan.FromSeconds(Math.Pow(2, retryCount)));
            _openAIConfig = openAIOptions.Value;
        }

        /// <summary>
        /// Allows for passing {PromptVariables.INPUT} to an OpenAI large language model and returning the model's reponse.
        /// </summary>
        /// <param name="input">The input string to pass to the large language model.</param>
        /// <param name="context">The chain execution context.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The output string.</returns>
        public override async Task<string> ExecuteAsync(string input, Dictionary<string, string> context, CancellationToken token = default)
        {
            using (var client = new HttpClient())
            {
                var modelName = _openAIConfig.ModelName;
                var apiVersion = "2023-03-15-preview";
                var domain = _openAIConfig.ApiEndpoint;
                var apiKey = _openAIConfig.ApiKey;
                var url = $"https://{domain}/openai/deployments/{modelName}/chat/completions?api-version={apiVersion}";
                var request = new
                {
                    messages = new dynamic[]
                    {
                        new
                        {
                            role = "user",
                            content = input
                        }
                    },
                    max_tokens = 8000,
                    temperature = 0.5,
                    frequency_penalty = 0,
                    presence_penalty = 0,
                    top_p = 0.95
                };
                var responseMessage = await _retryPolicy.ExecuteAsync(() =>
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                    _logger.LogDebug($"Request tokens ~: {input.Split(" ").Length * 1.4}");

                    requestMessage.Headers.Add("api-key", apiKey);
                    requestMessage.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                    return client.SendAsync(requestMessage, token);
                });
                var responseStr = await responseMessage.Content.ReadAsStringAsync(token);

                try
                {
                    if (!responseMessage.IsSuccessStatusCode) throw new ApplicationException(responseStr);

                    var response = JsonConvert.DeserializeObject<OpenAILLMResponse>(responseStr);

                    return response.Choices.First().Message.Content;
                }
                catch (Exception ex)
                {
                    Logging.Logs.Value = Logging
                        .Logs
                        .Value
                        .Concat(new List<IndentedLog>
                        {
                            new IndentedLog
                            {
                                Indentation = 0,
                                Text = $"Exception:{Environment.NewLine}{JsonConvert.SerializeObject(ex, Formatting.Indented)}"
                            }
                        })
                        .ToList();
                    throw new ArgumentOutOfRangeException("An error occured. If unsure, this is most likely due to the input model size being reached.", ex);
                }
            }
        }
    }
}
