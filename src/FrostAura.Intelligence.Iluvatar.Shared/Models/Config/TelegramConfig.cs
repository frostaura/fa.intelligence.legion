using System.Diagnostics;

namespace FrostAura.Intelligence.Iluvatar.Shared.Models.Config
{
	/// <summary>
	/// Telegram config model.
	/// </summary>
	[DebuggerDisplay("{BotName}")]
	public class TelegramConfig
	{
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
