using System.Diagnostics;

namespace FrostAura.Intelligence.Legion.Shared.Models.Config
{
	/// <summary>
	/// Telegram config model.
	/// </summary>
	[DebuggerDisplay("{BotIdentity}")]
	public class TelegramConfig
	{
		/// <summary>
		/// The identity that the bot should take.
		/// </summary>
		public string BotIdentity { get; set; }
		/// <summary>
		/// The access token for the bot.
		/// </summary>
		public string BotToken { get; set; }
		/// <summary>
		/// A personal chat ID that the bot should assume as the default user.
		/// </summary>
		public string PersonalChatId { get; set; }
        /// <summary>
        /// A default path to store received artifacts.
        /// </summary>
        public string MediaStoragePath { get; set; }
    }
}
