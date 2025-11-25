#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System;


var builder = WebApplication.CreateBuilder(args);

// Создаём TelegramClient через DI
builder.Services.AddSingleton(new TelegramBotClient(
    Environment.GetEnvironmentVariable("BOT_TOKEN") ?? ""));

var app = builder.Build();

app.MapPost("/bot-webhook", async (Update update, TelegramBotClient bot) =>
{
    if (update.Message == null || update.Message.Text == null)
        return Results.Ok();

    var text = update.Message.Text;
    long chatId = update.Message.Chat.Id;

    // Клавиатура снизу
    var bottomButtons = new ReplyKeyboardMarkup(new[]
    {
        new KeyboardButton[] { "FAQ" },
        new KeyboardButton[] { "Поддержка" },
        new KeyboardButton[] { "Канал" }
    })
    {
        ResizeKeyboard = true
    };

    // При /start
    if (text == "/start")
    {
        var startKeyboard = new ReplyKeyboardMarkup(
            KeyboardButton.WithWebApp(
                "Открыть приложение",
                new WebAppInfo
                {
                    Url = "https://titwolf.github.io/fit-app/"
                })
        )
        {
            ResizeKeyboard = true
        };

        await bot.SendTextMessageAsync(chatId,
            "Добро пожаловать! Нажмите кнопку, чтобы открыть приложение.",
            replyMarkup: startKeyboard);

        await bot.SendTextMessageAsync(chatId,
            "Выберите действие:",
            replyMarkup: bottomButtons);

        return Results.Ok();
    }

    // Обработка кнопок
    switch (text.ToLower())
    {
        case "faq":
            await bot.SendTextMessageAsync(chatId,
                "FitPlan — приложение для составления и отслеживания тренировок.");
            break;

        case "поддержка":
            await bot.SendTextMessageAsync(chatId, "Чат поддержки: @fapSupport");
            break;

        case "канал":
            await bot.SendTextMessageAsync(chatId, "Канал: https://t.me/fitappplan");
            break;
    }

    return Results.Ok();
});

// Просто проверка запуска
app.MapGet("/", () => "Bot is running.");

app.Run();
