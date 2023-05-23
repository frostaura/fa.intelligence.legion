namespace FrostAura.Intelligence.Iluvatar.Telegram.Extensions.String
{
    /// <summary>
    /// Extensions for managing Telegram messaging strings.
    /// </summary>
    public static class TelegramStrings
    {
        public static string MarkdownV2Escape(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;

            return str
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
    }
}
