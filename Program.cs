#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Подключаем TelegramBotClient через DI
builder.Services.AddSingleton(new TelegramBotClient(
    Environment.GetEnvironmentVariable("BOT_TOKEN") ?? throw new Exception("BOT_TOKEN не задан")));

// Создаём приложение
var app = builder.Build();

// Эндпоинт для webhook
app.MapPost("/bot-webhook", async (HttpRequest request, TelegramBotClient bot) =>
{
    Update? update;
    try
    {
        update = await JsonSerializer.DeserializeAsync<Update>(request.Body);
        if (update == null || update.Message == null || update.Message.Text == null)
            return Results.Ok();
    }
    catch
    {
        return Results.Ok();
    }

    var chatId = update.Message.Chat.Id;
    var text = update.Message.Text;

    var keyboard = new ReplyKeyboardMarkup(new[]
    {
        new KeyboardButton[] { "/faq" },
        new KeyboardButton[] { "/support" },
        new KeyboardButton[] { "/channel" }
    })
    {
        ResizeKeyboard = true
    };

    switch (text.ToLower())
    {
        case "/start":
            await bot.SendTextMessageAsync(chatId, "Добро пожаловать! Выберите команду:", replyMarkup: keyboard);
            break;
        case "/faq":
            await bot.SendTextMessageAsync(chatId, "FitPlan — приложение для составления и отслеживания тренировок.");
            break;
        case "/support":
            await bot.SendTextMessageAsync(chatId, "Чат поддержки: @fapSupport");
            break;
        case "/channel":
            await bot.SendTextMessageAsync(chatId, "Канал: https://t.me/fitappplan");
            break;
    }

    return Results.Ok();
});


// Проверка работы сервиса
app.MapGet("/", () => "Bot is running.");

// Запуск сервера
app.Run();
