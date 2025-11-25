#nullable enable
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

var builder = WebApplication.CreateBuilder(args);

// Подключаем TelegramBotClient через DI
builder.Services.AddSingleton(new TelegramBotClient(
    Environment.GetEnvironmentVariable("BOT_TOKEN") ?? ""));

// Подключаем сервис бота
builder.Services.AddSingleton<BotService>();

var app = builder.Build();

// Вебхук Telegram
app.MapPost("/bot-webhook", async ([FromBody] Update update, BotService botService) =>
{
    await botService.HandleUpdate(update);
    return Results.Ok();
});

// Проверка, что сервер работает
app.MapGet("/", () => "Bot is running.");

app.Run();
