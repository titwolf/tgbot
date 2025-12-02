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
app.MapPost("/bot-webhook", async ([FromBody] Update update, [FromServices] TelegramBotClient bot) =>
{
    if (update.Message == null || update.Message.Text == null)
        return Results.Ok();

    var chatId = update.Message.Chat.Id;
    var text = update.Message.Text;

    // Кнопки снизу
    var buttons = new ReplyKeyboardMarkup(new[]
    {
        new KeyboardButton[] { "/faq", "/support", "/channel" }
    })
    { ResizeKeyboard = true };

    if (text == "/start")
    {
        await bot.SendTextMessageAsync(chatId, "Добро пожаловать! Выберите команду:", replyMarkup: buttons);
        return Results.Ok();
    }

    switch (text.ToLower())
    {
        case "/faq":
            await bot.SendTextMessageAsync(chatId, "FitAppPlan — приложение для составления и отслеживания тренировок.");
            break;
        case "/support":
            await bot.SendTextMessageAsync(chatId, "Чат поддержки: @fapSupport");
            break;
        case "/channel":
            await bot.SendTextMessageAsync(chatId, "Канал: https://t.me/fitappplan");
            break;
        default:
            await bot.SendTextMessageAsync(chatId, "Неизвестная команда. Используйте кнопки ниже.", replyMarkup: buttons);
            break;
    }

    return Results.Ok();
});

// Проверка работы сервиса
app.MapGet("/", () => "Bot is running.");

// Запуск сервера
app.Run();
