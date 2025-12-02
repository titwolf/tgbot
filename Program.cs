#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

var builder = WebApplication.CreateBuilder(args);

// Бот из переменной окружения
var token = Environment.GetEnvironmentVariable("BOT_TOKEN");
builder.Services.AddSingleton(new TelegramBotClient(token!));

var app = builder.Build();

// Webhook endpoint
app.MapPost("/bot-webhook", async (Update update, TelegramBotClient bot) =>
{
    if (update.Message == null || update.Message.Text == null)
        return Results.Ok();

    var text = update.Message.Text.Trim().ToLower();
    var chatId = update.Message.Chat.Id;

    // нижние кнопки
    var keyboard = new ReplyKeyboardMarkup(new[]
    {
        new KeyboardButton[] { "faq" },
        new KeyboardButton[] { "support" },
        new KeyboardButton[] { "channel" }
    })
    {
        ResizeKeyboard = true
    };

    if (text == "/start")
    {
        await bot.SendTextMessageAsync(chatId,
            "Добро пожаловать! Выберите команду:",
            replyMarkup: keyboard);

        return Results.Ok();
    }

    switch (text)
    {
        case "faq":
        case "/faq":
            await bot.SendTextMessageAsync(chatId,
                "FAQ: это тренировочное приложение FitPlan.");
            break;

        case "support":
        case "/support":
            await bot.SendTextMessageAsync(chatId,
                "Поддержка: @fapSupport");
            break;

        case "channel":
        case "/channel":
            await bot.SendTextMessageAsync(chatId,
                "Наш канал: https://t.me/fitappplan");
            break;
    }

    return Results.Ok();
});

// Проверка
app.MapGet("/", () => "Bot is running!");

app.Run();
