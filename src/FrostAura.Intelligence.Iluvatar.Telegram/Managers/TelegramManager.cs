using FrostAura.Intelligence.Iluvatar.Shared.Models.Config;
using FrostAura.Intelligence.Iluvatar.Telegram.Extensions.String;
using FrostAura.Libraries.Semantic.Core.Enums.Models;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Models.Prompts;
using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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
        /// Allows for passing {PromptVariables.INPUT} to an OpenAI large language model and returning the model's reponse.
        /// </summary>
        private readonly LanguageModelThoughts _llmSkill;
        /// <summary>
        /// Cancellation token source to allow for cancelling downstream operations at any point.
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource { get; set; }
        /// <summary>
        /// A collection of conversations for all user threats.
        /// </summary>
        private readonly Dictionary<string, Conversation> _conversations = new Dictionary<string, Conversation>();

        /// <summary>
        /// Overloaded constructor for injecting dependencies.
        /// </summary>
        /// <param name="telegramOptions">Telegram app & bot configuration options.</param>
        /// <param name="logger">Logger instance.</param>
        public TelegramManager(IServiceProvider serviceProvider, IOptions<TelegramConfig> telegramOptions, ILogger<TelegramManager> logger)
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
            _llmSkill = serviceProvider.GetThoughtByName<LanguageModelThoughts>(nameof(LanguageModelThoughts));

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
            var isOnlinePrompt = "Write a short greetings message.";
            var isOnlineMessage = await _llmSkill.PromptSmallLLMAsync(isOnlinePrompt, token);

            await bot.SendTextMessageAsync(_telegramConfig.PersonalChatId, isOnlineMessage);
            bot.StartReceiving(async (bot, update, token) =>
            {
                try
                {
                    await OnMessageAsync(bot, update, token);
                }
                catch (Exception ex)
                {
                    var errorPrompt = $"Give me a message where you say that an error occured and the JSON is to follow.";
                    var errorMessage = await _llmSkill.PromptSmallLLMAsync(errorPrompt, token);
                    var exceptionMsg = $"{errorMessage}{Environment.NewLine}```json{Environment.NewLine}{JsonConvert.SerializeObject(ex, Formatting.Indented)}{Environment.NewLine}```";
                    await bot.SendTextMessageAsync(_telegramConfig.PersonalChatId, exceptionMsg.MarkdownV2Escape(), parseMode: ParseMode.MarkdownV2, replyToMessageId: update?.Message?.MessageId);
                    _logger.LogWarning($"[{this.GetType().Name}] Failed to process incoming message.", ex);
                }
            }, OnErrorAsync, _pollingOptions, _cancellationTokenSource.Token);
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
            var senderId = sender.Id.ToString();
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

                if(!_conversations.ContainsKey(senderId))
                {
                    _conversations[senderId] = await _llmSkill.ChatAsync(message?.Text, ModelType.LargeLLM, token);
                }
                else
                {
                    await _conversations[senderId].ChatAsync(message?.Text, token);
                }

                await bot.SendTextMessageAsync(update.Message.Chat.Id, _conversations[senderId].LastMessage.MarkdownV2Escape(), parseMode: ParseMode.MarkdownV2, replyToMessageId: update.Message.MessageId);
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
