using FrostAura.Intelligence.Iluvatar.Core.Consts.Cognitive;
using FrostAura.Intelligence.Iluvatar.Core.Extensions.Cognitive;
using FrostAura.Intelligence.Iluvatar.Core.Extensions.String;
using FrostAura.Intelligence.Iluvatar.Core.Models.Planning;
using FrostAura.Intelligence.Iluvatar.Core.Skills.Cognitive;
using FrostAura.Intelligence.Iluvatar.Core.Skills.IO;
using FrostAura.Intelligence.Iluvatar.Shared.Models.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace FrostAura.Intelligence.Iluvatar.Core.Skills.Core
{
    /// <summary>
    /// Take in any problem or query, $INPUT, and generate a comprehensive step by step plan for how to solve for $INPUT.
    /// </summary>
    public class PlannerSkill : BaseSkill
    {
        /// <summary>
        /// The description of the expected input.
        /// </summary>
        public override string InputDescription => "A goal to accomplish, a problem to solve or a high-level action to perform or a query.";
        /// <summary>
        /// The function of the skill.
        /// </summary>
        public override string Function => $"Take in any problem or query, {PromptVariables.INPUT}, solves the problem recursively and return a response to the original {PromptVariables.INPUT}. A planner can be used to solve impossible queries and can always be used as a last resort if no other skill, not even a {nameof(LLMSkill)}, can help.";
        /// <summary>
        /// All available skills.
        /// </summary>
        protected readonly List<BaseSkill> _availableSkills;
        /// <summary>
        /// The steps that the planner should follow to produce the plan.
        /// </summary>
        private List<string> _plannerSteps = new List<string>
        {
            "The plan should be as comprehensive as possible. This means break down problems into their smallest solvable parts. Think of it like functional programming.",
            "From a <goal> create a <plan> as a series of <skill>.",
            "Before using any <skill> in a <plan>, check that it is present in the most recent [AVAILABLE SKILLS] list. If it is not, do not use it. Do not assume that any skill that was previously defined or used in another <plan> or in [EXAMPLES] is automatically available or compatible with the current <plan>.",
            "Only use skills that are required for the given goal.",
            "A <skill> has an 'input' and an 'output'.",
            "The 'output' from each <skill> is automatically passed as 'input' to the subsequent <skill>.",
            "'input' does not need to be specified if it consumes the 'output' of the previous <skill>.",
            "To save an 'output' from a <skill>, to pass into a future <skill>, use <skill.<<SkillName>> ... set-context-variable: \"\"<UNIQUE_VARIABLE_KEY>\"\"/>",
            "Strictly the following parameters / variables / arguments should be defined as attributes on the <skill>: 'name', 'input', 'reasoning', 'critisism', 'set-context-variable'. Never include any other arguments as attributes.",
            "All other parameters / variables / arguments should be represented by a <argument> node inside of the <skill> instead of an attribute on the <skill>. For example 'source', 'chunk-size' etc",
            "All <argument> should always be provided. If unsure, use suggested values if given or sensible defaults.",
            "You will also suggest any additional skills that could make the task easier in the future and could potentially be reused in other future queries. These suggestions go inside the <suggested-skills> node.",
            "When suggesting new skills, those skills should always be small components that can perform core tasks like HTTP calls, file manipulation, AI model inference etc. Never as large as an entire use case. We want these skills to be reusable by the PlannerSkill.",
            "Provide your reasoning as a \"reasoning\" attribute on the <plan> and <skill>.",
            "Provide constructive critisism as a \"critisism\" attribute on <plan> and <skill>."
        };
        /// <summary>
        /// Examples for the planner to derrive insight from.
        /// </summary>
        private List<string> _examples => new List<string>
        {
            """
            [AVAILABLE SKILLS]
                <skill name="WriterSkill.Summarize" function="Summarize a text blob." input="The text blob to summarize."/>
                <skill name="LanguageHelpers.TranslateTo" function="Translate a text blob from English to 'translate_to_language'." input="The text blob to translate." translate_to_language="The language to translate the input text to."/>
                <skill name="EmailConnector.LookupContactEmail" function="Look up a contact's email address." input="The identifier of the contact to look up."/>
                <skill name="EmailConnector.EmailTo" function="Send an email to an email address." input="The text of the email body." recipient="The recipient email address."/>
            [END AVAILABLE SKILLS]

            <goal>Summarize the input, then translate to japanese and email it to Martin. Also SMS it to him.</goal>
            <plan reasoning="..." critisism="...">
              <skill name="WriterSkill.Summarize" reasoning="..." critisism="...">
                <arguments>
                </arguments>
              </skill>
              <skill name="LanguageHelpers.TranslateTo" set-context-variable="TRANSLATED_TEXT" reasoning="..." critisism="...">
                <arguments>
                    <argument name="translate_to_language" value="Japanese" />
                </arguments>
              </skill>
              <skill name="EmailConnector.LookupContactEmail" input="Martin" set-context-variable="EMAIL_CONTACT_RESULT" reasoning="..." critisism="...">
                <arguments>
                </arguments>
              </skill>
              <skill name="EmailConnector.EmailTo" input="$TRANSLATED_TEXT" reasoning="..." critisism="...">
                <arguments>
                    <argument name="recipient" value="$EMAIL_CONTACT_RESULT" />
                </arguments>
              </skill>
            </plan>
            <suggested-skills>
                <skill name="MobileConnector.LookupContactPhone" input="Look up a contact's phone number." how-suggestion="I would log into the Outlook web portal using a technology like Selenium, navigate to the contacts list, search for the contact and grab the info from there programatically."/>
                <skill name="MobileConnector.SMSTo" input="The body of the text to SMS." recipient="The recipient SMS number for the message." how-suggestion="I would integrate with the Twilio API in order to send smses programatically."/>
            </suggested-skills>
            """,
            """
            [AVAILABLE SKILLS]
                <skill name="Math.Operation" function="Allows for performing a mathematical operation on two numbers." input="The first required number.", operator="The mathematical operator to use (* / + -)." number-two="The 2nd required number."/>
                <skill name="System.Messaging" function="Allows for senting updates / messages / feedback to the user." input="The text content to send to the user." summary="A shorter description of the input." />
            [END AVAILABLE SKILLS]

            <goal>What is 2 + 5? And now multiplied by 3?</goal>
            <plan reasoning="..." critisism="...">
                <skill name="Math.Operation" input="2" set-context-variable="RESULT_1" reasoning="..." critisism="...">
                    <arguments>
                        <argument name="operator" value="+" />
                        <argument name="number-two" value="5" />
                    </arguments>
                </skill>
                <skill name="System.Messaging" input="$RESULT_1" reasoning="2 + 5 = $RESULT_1" critisism="...">
                  <arguments>
                      <argument name="summary" value="2 + 5 = $RESULT_1" />
                  </arguments>
                </skill>
                <skill name="Math.Operation" input="$RESULT_1" set-context-variable="RESULT_2" reasoning="..." critisism="...">
                    <arguments>
                        <argument name="operator" value="*" />
                        <argument name="number-two" value="3" />
                    </arguments>
                </skill>
                <skill name="System.Messaging" input="$RESULT_2" reasoning="(2 + 5) * 3 = $RESULT_2" critisism="...">
                  <arguments>
                      <argument name="summary" value="(2 + 5) * 3 = $RESULT_2" />
                  </arguments>
                </skill>
            </plan>
            <suggested-skills>
            </suggested-skills>
            """,
            $"""
            [AVAILABLE SKILLS]
                {_availableSkills.ToPromptStrings().Aggregate((l, r) => $"{l}{Environment.NewLine}{r}")}
            [END AVAILABLE SKILLS]

            <goal>What is the general theme of the search results on the URL https://www.google.com.au/search?q=Thor?</goal>
            <plan reasoning="..." critisism="...">
              <skill name="Semantic.Skills.IO.HttpGetSkill" input="https://www.google.com.au/search?q=Thor" set-context-variable="URL_CONTENT" reasoning="..." critisism="...">
                <arguments>
                </arguments>
              </skill>
              <skill name="Semantic.Skills.Cognitive.LLMSkill" input="Give the following HTML from a Google search, What are some common themes among the search results?\n---\n$URL_CONTENT" reasoning="..." critisism="...">
                <arguments>
                </arguments>
              </skill>
            </plan>
            <suggested-skills>
                <skill name="Semantic.Skills.DataNormalization.HTML" input="$URL_CONTENT" how-suggestion="I would take in the raw HTML string and strip out the bits we don't need like scripts and perhaps the <head> tag..., Example:..."/>
            </suggested-skills>
            """,
            $"""
            [AVAILABLE SKILLS]
                {_availableSkills.OnlyInclude(typeof(HttpGetSkill), typeof(PlannerSkill), typeof(LLMSkill)).ToPromptStrings().Aggregate((l, r) => $"{l}{Environment.NewLine}{r}")}
            [END AVAILABLE SKILLS]

            <goal>Write me a book of 400 pages based on a currently trending topic. Make sure you AI-generate the cook cover art.</goal>
            <plan reasoning="..." critisism="...">
              <skill name="Semantic.Skills.IO.HttpGetSkill" input="https://duckduckgo.com/?q=trending+book+topics" set-context-variable="URL_CONTENT" reasoning="..." critisism="...">
                <arguments>
                </arguments>
              </skill>
              <skill name="Semantic.Skills.Core.PlannerSkill" input="Take the search results from the following HTML content and from it find the single highest currently trending book topic.\n---\n$URL_CONTENT" set-context-variable="BOOK_TOPIC" reasoning="We can rely on the planner skill to break down a problem to smaller components, when we don't yet have the available skills required." critisism="...">
                <arguments>
                </arguments>
              </skill>
              <skill name="Semantic.Skills.Cognitive.LLMSkill" input="Write me an outline for a book with the following topic: $BOOK_TOPIC" set-context-variable="BOOK_OUTLINE" reasoning="..." critisism="...">
                <arguments>
                </arguments>
              </skill>
              <skill name="Semantic.Skills.Core.PlannerSkill" input="Based on the following book outline, write each of the chapters for the book and return it as a single text blob.\n---\n$BOOK_OUTLINE" set-context-variable="COMPLETE_BOOK" reasoning="..." critisism="...">
                <arguments>
                </arguments>
              </skill>
              ...
            </plan>
            <suggested-skills>
            </suggested-skills>
            """,
            $"""
            [AVAILABLE SKILLS]
                {_availableSkills.OnlyInclude(typeof(RecallFromMemorySkill), typeof(LLMSkill)).ToPromptStrings().Aggregate((l, r) => $"{l}{Environment.NewLine}{r}")}
            [END AVAILABLE SKILLS]

            <goal>When did I get my drivers license? Tell me something else that's interesting about that day too.</goal>
            <plan reasoning="..." critisism="...">
              <skill name="Semantic.Skills.Cognitive.RecallFromMemorySkill" input="When did I get my drivers license?" set-context-variable="MEMORIES" reasoning="The question seems like something to look up in the knowledge base as a personal or domain topic." critisism="...">
                <arguments>
                    <argument name="{PromptVariables.MEMORY_COUNT}" value="10" />
                </arguments>
              </skill>
              <skill name="Semantic.Skills.Cognitive.LLMSkill" input="Given the following memories from a knowledge based, answer the following query and where possible, recite the source the information in your response. If the available information doesn't contain the answer, answer as if there were no memories provided. \n---memories---\n$MEMORIES\n---end-memories---\nQuery: When did I get my drivers license? Tell me something else that's interesting about that day too." reasoning="..." critisism="...">
                <arguments>
                </arguments>
              </skill>
            </plan>
            <suggested-skills>
            </suggested-skills>
            """
        };
        /// <summary>
        /// The foundational prompt to generate a plan for a query.
        /// </summary>
        private string _prompt => $"""
            Create an XML plan step by step, to satisfy the goal given.
            Never give any disclaimers like 'As an AI language model, I...' or 'I am not a...' in your response. I understand this already.
            To create a plan, follow these steps:

            [STEPS]
                {_plannerSteps.Select(ps => $"- {ps}").Aggregate((l, r) => $"{l}{Environment.NewLine}{r}")}
            [END STEPS]

            [EXAMPLES]
                {_examples.Select(ps => $"[EXAMPLE]{Environment.NewLine}{ps}{Environment.NewLine}[END EXAMPLE]").Aggregate((l, r) => $"{l}{Environment.NewLine}{r}")}
            [END EXAMPLES]

            [AVAILABLE SKILLS]
                {PromptVariables.AVAILABLE_SKILLS}
            [END AVAILABLE SKILLS]

            <goal>{PromptVariables.INPUT}</goal>
        """;
        /// <summary>
        /// Retry policy for when a plan fails to be parsed.
        /// </summary>
        private readonly AsyncRetryPolicy<Root> _getPlanPolicy;
        /// <summary>
        /// Allows for passing {PromptVariables.INPUT} to an OpenAI large language model and returning the model's reponse.
        /// </summary>
        private readonly BaseSkill _llmSkill;

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="serviceProvider">A service provider for the DI container.</param>
        /// <param name="llmSkill">Allows for passing {PromptVariables.INPUT} to an OpenAI large language model and returning the model's reponse.</param>
        /// <param name="logger">Logger instance.</param>
        public PlannerSkill(IServiceProvider serviceProvider, LLMSkill llmSkill, ILogger<PlannerSkill> logger)
            : base(logger)
        {
            _availableSkills = ((IEnumerable<BaseSkill>)serviceProvider.GetService(typeof(IEnumerable<BaseSkill>)))
                .Concat(new List<BaseSkill> { this, llmSkill })
                .ToList();
            _getPlanPolicy = Policy
                .Handle<Exception>()
                .OrResult<Root>(m => m == default)
                .WaitAndRetryAsync(3, retryCount => TimeSpan.FromSeconds(Math.Pow(2, retryCount)));
            _llmSkill = llmSkill;
        }

        /// <summary>
        /// Take in any problem or query, $INPUT, and generate a comprehensive step by step plan for how to solve for $INPUT.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="context">The chain execution context.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The output string.</returns>
        public override async Task<string> ExecuteAsync(string input, Dictionary<string, string> context, CancellationToken token = default)
        {
            var plan = await PlanAndExecuteGoalAsync(input, context, token);

            return plan.Output;
        }

        /// <summary>
        /// Recursively plan for a goal. With the possibility of nested planners to compartmentalize problems.
        /// </summary>
        /// <param name="goal">The goal that the planner should solve for.</param>
        /// <param name="context">The chain execution context.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The plan to solve for the goal. A nested plan, if applicable.</returns>
        public async Task<Plan> PlanAndExecuteGoalAsync(string goal, Dictionary<string, string> context, CancellationToken token = default)
        {
            context[PromptVariables.AVAILABLE_SKILLS] = _availableSkills
                .ToPromptStrings()
                .Aggregate((l, r) => $"{l}{Environment.NewLine}{r}");
            context[PromptVariables.INPUT] = goal;

            var llmPrompt = _prompt.Contextualize(context);
            var planner = await _getPlanPolicy.ExecuteAsync(async () =>
            {
                var llmResponse = await _llmSkill.RunAsync(llmPrompt, context, token);
                var planner = llmResponse
                    .PlannerFromXmlStr(goal);

                return planner;
            });

            // Execute each step in the plan sequentially.
            foreach (var step in planner.Plan.Steps)
            {
                // Keep the output of the plan current with the last step's output.
                planner.Plan.Output = await step.ExecuteStepAsync(context, this, _llmSkill, _availableSkills, token);
            }

            return planner.Plan;
        }
    }
}
