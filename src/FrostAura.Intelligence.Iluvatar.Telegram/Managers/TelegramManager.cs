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
        /// A collection of message handler operations in order to fascilitate concurrent message handling.
        /// </summary>
        private List<Task> _onMessageReceivedHandlingTasks = new List<Task>();

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

            _logger.LogInformation("[{Type}] Successfully initialized.", GetType().Name);
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
                _onMessageReceivedHandlingTasks = _onMessageReceivedHandlingTasks
                    .Where(t => !t.IsCompleted)
                    .ToList();
                _onMessageReceivedHandlingTasks.Append(Task.Run(async () =>
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

                        await bot.SendTextMessageAsync(update.Message.From.Id, $"🔴 {exceptionMsg.MarkdownV2Escape()}", parseMode: ParseMode.MarkdownV2, replyToMessageId: update?.Message?.MessageId);
                        _logger.LogError(ex, "[{Type}] Failed to process incoming message.", GetType().Name);
                    }
                }));
            }, OnErrorAsync, _pollingOptions, _cancellationTokenSource.Token);
        }

        /// <summary>
        /// Clean upmanaged resources up.
        /// </summary>
        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _logger.LogDebug("[{Type}] Successfully cleaned up unmanaged resources.", GetType().Name);
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

            _logger.LogInformation("[{Type}][{SenderFullName}] Received a new message: {MessageUpdate}", GetType().Name, senderFullName, update);

            // Ensure directories exist.
            if (!Directory.Exists(Path.Combine(_telegramConfig.MediaStoragePath)))
                Directory.CreateDirectory(Path.Combine(_telegramConfig.MediaStoragePath));
            if (!Directory.Exists(Path.Combine(_telegramConfig.MediaStoragePath, "photos")))
                Directory.CreateDirectory(Path.Combine(_telegramConfig.MediaStoragePath, "photos"));
            if (!Directory.Exists(Path.Combine(_telegramConfig.MediaStoragePath, "documents")))
                Directory.CreateDirectory(Path.Combine(_telegramConfig.MediaStoragePath, "documents"));
            if (!Directory.Exists(Path.Combine(_telegramConfig.MediaStoragePath, "voicenotes")))
                Directory.CreateDirectory(Path.Combine(_telegramConfig.MediaStoragePath, "voicenotes"));

            // Process Photo Messages
            if (message?.Photo != default)
            {
                var fileName = $"{Guid.NewGuid()}.png";
                var filePath = Path.Combine(_telegramConfig.MediaStoragePath, "photos", fileName);
                var stream = new FileStream(filePath, FileMode.CreateNew);

                await bot.GetInfoAndDownloadFileAsync(message?.Photo.Last().FileId, stream);
                _logger.LogInformation("[{Type}][{SenderFullName}] Saved photo to '{FilePath}'.", GetType().Name, senderFullName, filePath);
                filesToProcess.Add($"Generate a detailed description for a Dall-E 3 prompt using the following image: {filePath}");
            }

            // Process File Messages
            if (message?.Document != default)
            {
                var fileExtension = message.Document.FileName.Split(".").Last();
                var fileName = $"{Guid.NewGuid()}.{fileExtension}";
                var filePath = Path.Combine(_telegramConfig.MediaStoragePath, "documents", fileName);
                var stream = new FileStream(filePath, FileMode.CreateNew);

                await bot.GetInfoAndDownloadFileAsync(message?.Document.FileId, stream);
                _logger.LogInformation("[{Type}][{SenderFullName}] Saved document to '{FilePath}'.", GetType().Name, senderFullName, filePath);
                filesToProcess.Add($"Analyze the following file using your creative and logical thinking, using the tools at your disposal: {filePath}");
            }

            // Process Voice Messages
            if (message?.Voice != default)
            {
                var fileName = $"{Guid.NewGuid()}.wav";
                var filePath = Path.Combine(_telegramConfig.MediaStoragePath, "voicenotes", fileName);
                var stream = new FileStream(filePath, FileMode.CreateNew);

                await bot.GetInfoAndDownloadFileAsync(message?.Voice.FileId, stream);
                _logger.LogInformation("[{Type}][{SenderFullName}] Saved voicenote to '{FilePath}'.", GetType().Name, senderFullName, filePath);
                filesToProcess.Add($"Extract the intent from the following voicenote by using the tools at your disposal and respond to it accordingly as if it was a text query/question/prompt: {filePath}");
            }

            // Process Text Messages
            if (message?.Text != default)
            {
                _logger.LogInformation("[{Type}][{SenderFullName}] processing query: {MessageText}.", GetType().Name, senderFullName, message?.Text);

                var botResponseMessage = await bot.SendTextMessageAsync(update.Message.Chat.Id, "🟡 Thinking...".MarkdownV2Escape(), parseMode: ParseMode.MarkdownV2, replyToMessageId: update.Message.MessageId);
                var operationContext = new OperationContext
                {
                    Id = botResponseMessage.MessageId.ToString(),
                    Name = senderFullName
                };
                var normalizedMessageText = message?.Text.EscapeCodeStrings();

                if (!_conversations.ContainsKey(senderId))
                {
                    _conversations[senderId] = await _llmSkill.ChatAsync(normalizedMessageText, ModelType.LargeLLM, token, operationContext);
                }
                else
                {
                    await _conversations[senderId].ChatAsync(normalizedMessageText, token, operationContext);
                }

                // TODO: Delete old updates once successful response is done and send a fresh response. This will send a notification upon completion.
                await bot.EditMessageTextAsync(update.Message.Chat.Id, botResponseMessage.MessageId, "🟢 " + _conversations[senderId].LastMessage.MarkdownV2Escape(), parseMode: ParseMode.MarkdownV2);
            }
            else
            {
                _logger.LogInformation("[{Type}][{SenderFullName}] processing file(s).", GetType().Name, senderFullName);

                var botResponseMessage = await bot.SendTextMessageAsync(update.Message.Chat.Id, "🟡 Analyzing file(s)...".MarkdownV2Escape(), parseMode: ParseMode.MarkdownV2, replyToMessageId: update.Message.MessageId);
                var messageToSend = filesToProcess.Aggregate((l, r) => $"{l}{Environment.NewLine}{r}");

                if (!string.IsNullOrWhiteSpace(message?.Caption))
                    messageToSend += $"{Environment.NewLine}{Environment.NewLine}{message.Caption}";

                if (!_conversations.ContainsKey(senderId))
                {
                    _conversations[senderId] = await _llmSkill.ChatAsync(messageToSend, ModelType.LargeLLM, token);
                }
                else
                {
                    await _conversations[senderId].ChatAsync(messageToSend, token);
                }

                await bot.EditMessageTextAsync(update.Message.Chat.Id, botResponseMessage.MessageId, "🟢 " + _conversations[senderId].LastMessage.MarkdownV2Escape(), parseMode: ParseMode.MarkdownV2);
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
            _logger.LogError(ex, "[{Type}] Exception while polling for Telegram bot updates.", GetType().Name);

            return Task.CompletedTask;
        }
    }
}
