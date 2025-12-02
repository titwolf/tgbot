#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

var builder = WebApplication.CreateBuilder(args);

// Получаем токен из переменной окружения
string? token = Environment.GetEnvironmentVariable("BOT_TOKEN");

if (string.IsNullOrWhiteSpace(token))
    throw new Exception("BOT_TOKEN не найден!");

// Регистрируем TelegramBotClient
builder.Services.AddSingleton(new TelegramBotClient(token));

var app = builder.Build();

// Webhook endpoint
app.MapPost("/bot-webhook", async (Update update, TelegramBotClient bot) =>
{
    if (update.Message?.Text == null)
        return Results.Ok();

    long chatId = update.Message.Chat.Id;
    string text = update.Message.Text.ToLower();

    var keyboard = new ReplyKeyboardMarkup(new[]
    {
        new KeyboardButton[] { "/faq" },
        new KeyboardButton[] { "/support" },
        new KeyboardButton[] { "/channel" }
    })
    {
        ResizeKeyboard = true
    };

    switch (text)
    {
        case "/start":
            await bot.SendTextMessageAsync(chatId,
                "Бот работает! Выберите команду:",
                replyMarkup: keyboard);
            break;

        case "/faq":
            await bot.SendTextMessageAsync(chatId,
                "FAQ: Это простой бот, который отвечает на команды.");
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

    return Results.Ok();
});

// Проверка сервера
app.MapGet("/", () => "Bot is running");

app.Run();
