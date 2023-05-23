using FrostAura.Intelligence.Iluvatar.Core.Consts.Cognitive;
using FrostAura.Intelligence.Iluvatar.Core.Models.Embeddings.OpenAI;
using FrostAura.Intelligence.Iluvatar.Shared.Models.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace FrostAura.Intelligence.Iluvatar.Core.Skills.Cognitive
{
    /// <summary>
    /// Allows for passing {PromptVariables.INPUT} to an OpenAI embedding model and returning the embeddings for the input passed.
    /// </summary>
    public class EmbeddingSkill : BaseSkill
    {
        /// <summary>
        /// The description of the expected input.
        /// </summary>
        public override string InputDescription => "A text blob to generate embeddings for.";
        /// <summary>
        /// The function of the skill.
        /// </summary>
        public override string Function => $"Allows for passing {PromptVariables.INPUT} to an OpenAI embedding model to map high-dimensional data, such as text, into a lower-dimensional continuous vector space. The goal is to preserve semantic relationships and capture meaningful patterns within the data. In the context of natural language processing (NLP), embedding models transform words, phrases, or sentences into dense vectors, which can be used as input for various downstream tasks, such as classification, clustering, or information retrieval.";
        /// <summary>
        /// OpenAI configuration.
        /// </summary>
        private readonly OpenAIConfig _openAIConfig;

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="openAIOptions">OpenAI configuration options.</param>
        /// <param name="logger">Logger instance.</param>
        public EmbeddingSkill(IOptions<OpenAIConfig> openAIOptions, ILogger<EmbeddingSkill> logger)
            : base(logger)
        {
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
                var modelName = _openAIConfig.EmbeddingModelName;
                var apiVersion = "2023-03-15-preview";
                var domain = _openAIConfig.ApiEndpoint;
                var apiKey = _openAIConfig.ApiKey;
                var url = $"https://{domain}/openai/deployments/{modelName}/embeddings?api-version={apiVersion}";
                var request = new
                {
                    input = input
                };
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                requestMessage.Headers.Add("api-key", apiKey);
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                var responseMessage = await client.SendAsync(requestMessage, token);
                var responseStr = await responseMessage.Content.ReadAsStringAsync(token);
                var response = JsonConvert.DeserializeObject<OpenAIEmbeddingResponse>(responseStr);
                var resultStr = JsonConvert.SerializeObject(response.Data.First().Embedding);

                return resultStr;
            }
        }
    }
}
