using System.Collections.Generic;
using System.Reflection;
using FrostAura.Intelligence.Iluvatar.Core.Skills;
using FrostAura.Intelligence.Iluvatar.Core.Skills.Core;
using FrostAura.Intelligence.Iluvatar.Shared.Models.Config;
using FrostAura.Intelligence.Iluvatar.Telegram.Extensions.String;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FrostAura.Intelligence.Iluvatar.Telegram.Managers
{
    /// <summary>
    /// Telegram host manager. This is the entry point to the application.
    /// </summary>
    public class TelegramManager : IDisposable
    {
        /// <summary>
        /// Telegram app & bot configuration.
        /// </summary>
        private readonly TelegramConfig _telegramConfig;
        /// <summary>
        /// Telegram receiver polling options.
        /// </summary>
        private readonly ReceiverOptions _pollingOptions;
        /// <summary>
        /// Logger instance.
        /// </summary>
        private readonly ILogger _logger;
        /// <summary>
        /// The Iluvatar semantic planner skill.
        /// </summary>
        private readonly BaseSkill _plannerSkill;
        /// <summary>
        /// Cancellation token source to allow for cancelling downstream operations at any point.
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource { get; set; }

        /// <summary>
        /// Overloaded constructor for injecting dependencies.
        /// </summary>
        /// <param name="telegramOptions">Telegram app & bot configuration options.</param>
        /// <param name="plannerSkill">The Iluvatar semantic planner skill.</param>
        /// <param name="logger">Logger instance.</param>
        public TelegramManager(IOptions<TelegramConfig> telegramOptions, PlannerSkill plannerSkill, ILogger<TelegramManager> logger)
        {
            _telegramConfig = telegramOptions.Value;
            _logger = logger;
            _pollingOptions = new ReceiverOptions
            {
                ThrowPendingUpdates = true,
                AllowedUpdates = new[]
                {
                    UpdateType.CallbackQuery,
                    UpdateType.ChannelPost,
                    UpdateType.ChatJoinRequest,
                    UpdateType.ChatMember,
                    UpdateType.ChosenInlineResult,
                    UpdateType.EditedChannelPost,
                    UpdateType.EditedMessage,
                    UpdateType.InlineQuery,
                    UpdateType.Message,
                    UpdateType.MyChatMember,
                    UpdateType.Poll,
                    UpdateType.PollAnswer,
                    UpdateType.PreCheckoutQuery,
                    UpdateType.ShippingQuery,
                    UpdateType.Unknown
                }
            };
            _plannerSkill = plannerSkill;

            _logger.LogInformation($"[{this.GetType().Name}] Successfully initialized.");
        }

        /// <summary>
        /// Run the Telegram host.
        /// </summary>
        /// <param name="token">Allow for cancelling downsteam operations.</param>
        /// <returns>Void</returns>
        public async Task RunAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            var bot = new TelegramBotClient(_telegramConfig.BotToken);
            var me = await bot.GetMeAsync(_cancellationTokenSource.Token);

            bot.StartReceiving((bot, update, token) => OnMessageAsync(bot, update, token), OnErrorAsync, _pollingOptions, _cancellationTokenSource.Token);
        }

        /// <summary>
        /// Clean upmanaged resources up.
        /// </summary>
        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _logger.LogInformation($"[{this.GetType().Name}] Successfully cleaned up unmanaged resources.");
        }

        /// <summary>
        /// Handle incoming messages.
        /// </summary>
        /// <param name="bot">Telegram bot instance.</param>
        /// <param name="update">The update context.</param>
        /// <param name="token">Allow for cancelling downsteam operations.</param>
        /// <returns>Void</returns>
        private async Task OnMessageAsync(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            var message = update.Message;
            var sender = message?.From;
            var senderFullName = $"{sender?.FirstName} {sender?.LastName}";
            var filesToProcess = new List<string>();

            _logger.LogInformation($"[{this.GetType().Name}][{senderFullName}] Received a new message: {update}");

            // Process Photo Messages
            if (message?.Photo != default)
            {
                var fileName = $"{Guid.NewGuid()}.png";
                var filePath = Path.Combine(_telegramConfig.MediaStoragePath, "photos", fileName);
                var stream = new FileStream(filePath, FileMode.CreateNew);

                await bot.GetInfoAndDownloadFileAsync(message?.Photo.Last().FileId, stream);
                _logger.LogInformation($"[{this.GetType().Name}][{senderFullName}] Saved photo to '{filePath}'.");
                filesToProcess.Add(filePath);
            }

            // Process File Messages
            if (message?.Document != default)
            {
                var fileName = message.Document.FileName;
                var filePath = Path.Combine(_telegramConfig.MediaStoragePath, "documents", fileName);
                var stream = new FileStream(filePath, FileMode.CreateNew);

                await bot.GetInfoAndDownloadFileAsync(message?.Document.FileId, stream);
                _logger.LogInformation($"[{this.GetType().Name}][{senderFullName}] Saved document to '{filePath}'.");
                filesToProcess.Add(filePath);
            }

            // Process Voice Messages
            if (message?.Voice != default)
            {
                var fileName = $"{Guid.NewGuid()}.mp3";
                var filePath = Path.Combine(_telegramConfig.MediaStoragePath, "voicenotes", fileName);
                var stream = new FileStream(filePath, FileMode.CreateNew);

                await bot.GetInfoAndDownloadFileAsync(message?.Voice.FileId, stream);
                _logger.LogInformation($"[{this.GetType().Name}][{senderFullName}] Saved voicenote to '{filePath}'.");
                filesToProcess.Add(filePath);
            }

            // Process Text Messages
            if (message?.Text != default)
            {
                _logger.LogInformation($"[{this.GetType().Name}][{senderFullName}] processing query: {message?.Text}.");

                var response = await _plannerSkill.ExecuteAsync(message?.Text, new Dictionary<string, string>());

                await bot.SendTextMessageAsync(update.Message.Chat.Id, response.MarkdownV2Escape(), parseMode: ParseMode.MarkdownV2, replyToMessageId: update.Message.MessageId);
            }
        }

        /// <summary>
        /// Handle Telegram errors.
        /// </summary>
        /// <param name="bot">Telegram bot instance.</param>
        /// <param name="ex">Exception / error context.</param>
        /// <param name="token">Allow for cancelling downsteam operations.</param>
        /// <returns>Void</returns>
        private Task OnErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken token)
        {
            _logger.LogError(ex, $"[{this.GetType().Name}] Exception while polling for Telegram bot updates.");

            return Task.CompletedTask;
        }
    }
}
