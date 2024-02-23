using FrostAura.Libraries.Core.Extensions.Validation;

namespace FrostAura.Intelligence.Iluvatar.Telegram.Extensions.String
{
    /// <summary>
    /// Extensions for managing Telegram messaging strings.
    /// </summary>
    public static class TelegramStrings
    {
        public static string MarkdownV2Escape(this string str)
        {
            return str
                .ThrowIfNullOrWhitespace(nameof(str))
                .Replace("_", "\\_")
                .Replace("[", "\\[")
                .Replace("]", "\\]")
                .Replace("(", "\\(")
                .Replace(")", "\\)")
                .Replace("~", "\\~")
                .Replace(">", "\\>")
                .Replace("#", "\\#")
                .Replace("+", "\\+")
                .Replace("-", "\\-")
                .Replace("=", "\\=")
                .Replace("|", "\\|")
                .Replace("{", "\\{")
                .Replace("}", "\\}")
                .Replace(".", "\\.")
                .Replace("!", "\\!");
        }

        public static string EscapeCodeStrings(this string str)
        {
            return str
                .ThrowIfNullOrWhitespace(nameof(str))
                .Replace("`", "");
        }
    }
}
