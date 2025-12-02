using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

var builder = WebApplication.CreateBuilder(args);

// Подключаем TelegramBotClient через DI
builder.Services.AddSingleton(new TelegramBotClient(
    Environment.GetEnvironmentVariable("BOT_TOKEN") ?? ""
));

var app = builder.Build();

// Webhook endpoint
app.MapPost("/bot-webhook", async (HttpRequest request, TelegramBotClient bot) =>
{
    Update? update;
    try
    {
        update = await JsonSerializer.DeserializeAsync<Update>(request.Body);
        if (update?.Message?.Text == null)
            return Results.Ok();
    }
    catch
    {
        return Results.Ok(); // Любой некорректный JSON игнорируем
    }

    var chatId = update.Message.Chat.Id;
    var text = update.Message.Text.ToLower();

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

// Простой тестовый endpoint
app.MapGet("/", () => "Bot is running.");

app.Run();
