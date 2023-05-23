namespace FrostAura.Intelligence.Iluvatar.Core.Extensions.Cognitive
{
    /// <summary>
    /// Extension methods for prompts.
    /// </summary>
    public static class PromptExtensions
    {
        /// <summary>
        /// Contextualize / interpolate all variables that are available in the context.
        /// </summary>
        /// <param name="template">The prompt template to interpolate.</param>
        /// <param name="context">The context to extract variables to interpolate from.</param>
        /// <returns>The interpolated prompt.</returns>
        public static string Contextualize(this string template, Dictionary<string, string> context)
        {
            var response = template;

            foreach (var item in context)
            {
                response = response.Replace(item.Key, item.Value);
            }

            return response;
        }
    }
}
