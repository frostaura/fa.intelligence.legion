using FrostAura.AI.Legion.Enums.Communication;
using FrostAura.AI.Legion.Models.Common;
using FrostAura.AI.Legion.Models.Communication;

namespace FrostAura.AI.Legion.Extensions.Models;

/// <summary>
/// Extensions for messaging content.
/// </summary>
public static class ContentExtensions
{
	/// <summary>
	/// Parse a simple text query as a Legion request.
	/// </summary>
	/// <param name="query">The simple text / string query.</param>
	/// <returns>A legion request.</returns>
	public static LegionRequest ToLegionRequest(this string query)
	{
		return new LegionRequest
		{
			Content =
			{
				new MessageContent
				{
					ContentType = ContentType.Text,
					Content = query
				}
			}
		};
	}
}
