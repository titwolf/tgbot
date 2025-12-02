#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

var builder = WebApplication.CreateBuilder(args);

// токен из переменной среды
var token = Environment.GetEnvironmentVariable("BOT_TOKEN");
var webhookUrl = Environment.GetEnvironmentVariable("WEBHOOK_URL");

if (string.IsNullOrEmpty(token))
    throw new Exception("BOT_TOKEN is not set");

if (string.IsNullOrEmpty(webhookUrl))
    throw new Exception("WEBHOOK_URL is not set");

// создаем клиента
var bot = new TelegramBotClient(token);

// включаем webhook
await bot.SetWebhookAsync(webhookUrl);

var app = builder.Build();

// корень сайта
app.MapGet("/", () => "Bot is running");

// обработчик webhook
app.MapPost("/bot-webhook", async (Update update) =>
{
    if (update.Message == null || update.Message.Text == null)
        return;

    long chatId = update.Message.Chat.Id;
    string text = update.Message.Text.Trim().ToLower();

    var buttons = new ReplyKeyboardMarkup(new[]
    {
        new KeyboardButton[] { "/faq" },
        new KeyboardButton[] { "/support" },
        new KeyboardButton[] { "/channel" }
    })
    {
        ResizeKeyboard = true
    };

    if (text == "/start")
    {
        await bot.SendTextMessageAsync(chatId,
            "Привет! Выбери действие:", replyMarkup: buttons);
        return;
    }

    switch (text)
    {
        case "/faq":
            await bot.SendTextMessageAsync(chatId,
                "FAQ: FitPlan — приложение для тренировок.");
            break;

        case "/support":
            await bot.SendTextMessageAsync(chatId,
                "Поддержка: @fapSupport");
            break;

        case "/channel":
            await bot.SendTextMessageAsync(chatId,
                "Канал: https://t.me/fitappplan");
            break;
    }
});

app.Run();
