using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

public class BotService
{
    private readonly ITelegramBotClient _bot;
    private readonly ILogger<BotService> _logger;

    public BotService(ITelegramBotClient bot, ILogger<BotService> logger)
    {
        _bot = bot;
        _logger = logger;
    }

    /// <summary>
    /// Инициализация: выставляем webhook (если указана переменная WEBHOOK_URL).
    /// WEBHOOK_URL должен быть вида https://your-app.onrender.com
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            string? webhookBase = Environment.GetEnvironmentVariable("WEBHOOK_URL");
            if (string.IsNullOrWhiteSpace(webhookBase))
            {
                _logger.LogWarning("WEBHOOK_URL не задан. Не устанавливаю webhook. Пожалуйста, задайте WEBHOOK_URL env var (https://your-app.onrender.com).");
                return;
            }

            var webhookUrl = $"{webhookBase.TrimEnd('/')}/bot-webhook";
            await _bot.SetWebhookAsync(webhookUrl);
            _logger.LogInformation("Webhook установлен: {0}", webhookUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Не удалось установить webhook.");
            throw;
        }
    }

    /// <summary>
    /// Обработка входящего обновления (вызывается из контроллера).
    /// </summary>
    public async Task HandleUpdateAsync(Update update)
    {
        try
        {
            if (update.Message == null || update.Message.Text == null) return;

            var msg = update.Message;
            var text = msg.Text.Trim();

            // Кнопки под полем ввода
            var bottomKeyboard = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "FAQ" },
                new KeyboardButton[] { "Поддержка" },
                new KeyboardButton[] { "Канал" }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };

            if (text.Equals("/start", StringComparison.OrdinalIgnoreCase))
            {
                // Кнопка открытия WebApp размещаем в первом сообщении (заместо меню)
                var startKeyboard = new ReplyKeyboardMarkup(KeyboardButton.WithWebApp(
                    "Открыть приложение",
                    new Telegram.Bot.Types.WebApp.WebAppInfo { Url = Environment.GetEnvironmentVariable("APP_URL") ?? "https://example.com" }
                ))
                {
                    ResizeKeyboard = true
                };

                await _bot.SendTextMessageAsync(msg.Chat.Id,
                    "Добро пожаловать! Нажмите кнопку «Открыть приложение», либо выберите действие ниже.",
                    replyMarkup: startKeyboard);

                // Отдельным сообщением показываем нижние кнопки (FAQ/Поддержка/Канал)
                await _bot.SendTextMessageAsync(msg.Chat.Id,
                    "Выберите действие:",
                    replyMarkup: bottomKeyboard);

                return;
            }

            // Обработка команд и кнопок
            switch (text.ToLower())
            {
                case "/support":
                case "поддержка":
                    await _bot.SendTextMessageAsync(msg.Chat.Id, "Чат поддержки: @fapSupport", replyMarkup: bottomKeyboard);
                    break;

                case "/channel":
                case "канал":
                    await _bot.SendTextMessageAsync(msg.Chat.Id, "Канал новостей: https://t.me/fitappplan", replyMarkup: bottomKeyboard);
                    break;

                case "/faq":
                case "faq":
                    await _bot.SendTextMessageAsync(msg.Chat.Id,
                        "Это приложение для создания и отслеживания тренировок.\n\n" +
                        "Вы можете:\n" +
                        "- Создавать тренировки\n" +
                        "- Вести учёт дней занятий\n" +
                        "- Просматривать свои тренировки\n\n" +
                        "Приложение запускается через WebApp (кнопка «Открыть приложение»).",
                        replyMarkup: bottomKeyboard);
                    break;

                default:
                    // Игнорируем или можно отправить подсказку
                    // await _bot.SendTextMessageAsync(msg.Chat.Id, "Неизвестная команда. Используйте /faq, /support или /channel.", replyMarkup: bottomKeyboard);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке update.");
        }
    }
}
