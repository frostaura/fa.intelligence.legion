using FrostAura.Intelligence.Iluvatar.Core.Attributes.Skills;
using FrostAura.Intelligence.Iluvatar.Core.Consts.Cognitive;
using FrostAura.Intelligence.Iluvatar.Core.Extensions.String;
using FrostAura.Intelligence.Iluvatar.Core.Models.Memory;
using FrostAura.Intelligence.Iluvatar.Shared.Models.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace FrostAura.Intelligence.Iluvatar.Core.Skills.Cognitive
{
    /// <summary>
    /// Allows for passing {PromptVariables.INPUT}, a text blob that should be used to query a knowledge base (typically a vector database like Pinecone).
    /// </summary>
    public class RecallFromMemorySkill : BaseSkill
    {
        /// <summary>
        /// The description of the expected input.
        /// </summary>
        public override string InputDescription => "A text blob to query the knowledge database with for similar memories to this query.";
        /// <summary>
        /// The function of the skill.
        /// </summary>
        public override string Function => $"Allows for passing {PromptVariables.INPUT}, a text blob that should be used to query the knowledge base (typically a vector database like Pinecone)";
        /// <summary>
        /// The embedding skill.
        /// </summary>
        private readonly BaseSkill _embeddingSkill;
        /// <summary>
        /// Pinecone configuration.
        /// </summary>
        private readonly PineconeConfig _pineconeConfig;

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="pineconeOptions">Pinecone config options.</param>
        /// <param name="embeddingSkill">The embedding skill.</param>
        /// <param name="logger">Logger instance.</param>
        public RecallFromMemorySkill(IOptions<PineconeConfig> pineconeOptions, EmbeddingSkill embeddingSkill, ILogger<RecallFromMemorySkill> logger)
            : base(logger)
        {
            _embeddingSkill = embeddingSkill;
            _pineconeConfig = pineconeOptions.Value;
        }

        /// <summary>
        /// Allows for passing {PromptVariables.INPUT} to an OpenAI large language model and returning the model's reponse.
        /// </summary>
        /// <param name="input">The input string to pass to the large language model.</param>
        /// <param name="context">The chain execution context.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The output string.</returns>
        [SkillRequiredContextVariable(PromptVariables.MEMORY_COUNT, $"The count of most similar memories (topK - in Pinecone terms) to fetch from the knowledge base. Use a default value of 10, if unsure.")]
        public override async Task<string> ExecuteAsync(string input, Dictionary<string, string> context, CancellationToken token = default)
        {
            var memoryCount = int.Parse(context[PromptVariables.MEMORY_COUNT]);
            var queryEmbedding = JsonConvert.DeserializeObject<List<double>>(await _embeddingSkill.RunAsync(input, context, token));
            var pineconeEnvironment = _pineconeConfig.Environment;
            var pineconeApiKey = _pineconeConfig.ApiKey;
            var pineconeIndexName = _pineconeConfig.IndexName;
            var pineconeNamespace = _pineconeConfig.Namespace;
            var url = $"https://{pineconeIndexName}.svc.{pineconeEnvironment}.pinecone.io/query";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Api-Key", pineconeApiKey);

                var data = new
                {
                    includeValues = false,
                    includeMetadata = true,
                    vector = queryEmbedding,
                    topK = memoryCount,
                    @namespace = pineconeNamespace
                };

                var jsonContent = JsonConvert.SerializeObject(data);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content, token);
                var responseContent = await response.Content.ReadAsStringAsync(token);

                if (response.IsSuccessStatusCode)
                {
                    return responseContent;
                }
                else
                {
                    throw new ExecutionEngineException($"Failed to recall memories from Pinecone.{Environment.NewLine}{responseContent}");
                }
            }
        }

        /// <summary>
        /// Get a collection of memories, given a text blob.
        /// </summary>
        /// <param name="text">Text blob to get memories from.</param>
        /// <param name="context">The chain execution context.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>A collection of memories for the text blob.</returns>
        private async Task<List<Memory>> GetMemoriesFromTextBlobAsync(string text, Dictionary<string, string> context, CancellationToken token)
        {
            var source = context[PromptVariables.SOURCE];
            var chunkSize = int.Parse(context[PromptVariables.CHUNK_SIZE]);
            var overlapSize = int.Parse(context[PromptVariables.OVERLAP_SIZE]);
            var chunks = text.ToChunks(chunkSize, overlapSize);
            var memoriesTasks = new List<Task<Memory>>();

            for (int i = 0; i < chunks.Count; i++)
            {
                memoriesTasks.Add(GetMemoryFromChunkAsync(chunks[i], context, token));
            }

            var response = await Task.WhenAll(memoriesTasks);

            for (int i = 0; i < response.Length; i++)
            {
                var memory = response[i];

                memory.Metadata.ChunkOverlap = overlapSize;
                memory.Metadata.ChunkSize = chunkSize;
                memory.Metadata.IndexInChunk = i;
                memory.Metadata.Source = source;
            }

            return response
                .ToList();
        }

        /// <summary>
        /// Get the memory representation for a chunk.
        /// </summary>
        /// <param name="text">The text content of the chunk.</param>
        /// <param name="context">The chain execution context.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The memory representation of the chunk.</returns>
        private async Task<Memory> GetMemoryFromChunkAsync(string text, Dictionary<string, string> context, CancellationToken token)
        {
            var meta = new MemoryMetadata
            {
                Text = text
            };
            var embedding = JsonConvert.DeserializeObject<List<double>>(await _embeddingSkill.RunAsync(text, context, token));
            var memory = new Memory
            {
                Metadata = meta,
                Embeddings = embedding
            };

            return memory;
        }
    }
}
