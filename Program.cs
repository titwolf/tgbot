#nullable enable
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// Логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Telegram client (берёт токен из переменных окружения)
string botToken = Environment.GetEnvironmentVariable("BOT_TOKEN") ?? "";
if (string.IsNullOrWhiteSpace(botToken))
{
    Console.WriteLine("Ошибка: переменная окружения BOT_TOKEN не задана. Установите токен в настройках Render.");
}

// Регистрируем TelegramBotClient и BotService
builder.Services.AddSingleton<ITelegramBotClient>(sp => new TelegramBotClient(botToken));
builder.Services.AddSingleton<BotService>();

// Контроллеры + JSON options для корректной десериализации Update
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

var app = builder.Build();

// Инициализация webhook (после DI построения)
var logger = app.Services.GetRequiredService<ILogger<Program>>();
var botService = app.Services.GetRequiredService<BotService>();
try
{
    await botService.InitializeAsync();
    logger.LogInformation("BotService initialized.");
}
catch (Exception ex)
{
    logger.LogError(ex, "Ошибка при инициализации BotService.");
}

// Map controllers
app.MapControllers();

// Простая проверка работоспособности
app.MapGet("/", () => "Bot service is running.");

// Запуск
app.Run();
