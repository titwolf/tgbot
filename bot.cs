#nullable enable
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var builder = WebApplication.CreateBuilder(args);

// Получаем токен из переменной окружения
string? token = Environment.GetEnvironmentVariable("BOT_TOKEN");
if (string.IsNullOrEmpty(token))
{
    Console.WriteLine("BOT_TOKEN не найден");
    return;
}

var bot = new TelegramBotClient(token);

builder.Services.AddSingleton(bot);
builder.Services.AddControllers().AddNewtonsoftJson();

var app = builder.Build();

string webhookUrl = $"{Environment.GetEnvironmentVariable("RENDER_EXTERNAL_URL")}/bot-webhook";

await bot.DeleteWebhookAsync();
await bot.SetWebhookAsync(webhookUrl);

Console.WriteLine("Бот запущен по webhook");
Console.WriteLine("Webhook URL: " + webhookUrl);

app.MapPost("/bot-webhook", async (Update update, TelegramBotClient botClient) =>
{
    if (update.Message != null)
    {
        var msg = update.Message;

        // ----- КНОПКИ -----
        var buttons = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "FAQ", "Поддержка", "Канал" }
        })
        {
            ResizeKeyboard = true
        };

        if (msg.Text == "/start")
        {
            await botClient.SendTextMessageAsync(
                chatId: msg.Chat.Id,
                text: "Добро пожаловать!",
                replyMarkup: buttons
            );
            return;
        }

        if (msg.Text == "FAQ")
        {
            await botClient.SendTextMessageAsync(
                msg.Chat.Id,
                "FitPlan — приложение для создания и ведения тренировок.\nВы можете создавать собственные тренировки, отслеживать прогресс и сохранять историю."
            );
            return;
        }

        if (msg.Text == "Поддержка")
        {
            await botClient.SendTextMessageAsync(msg.Chat.Id, "Чат поддержки: @fapSupport");
            return;
        }

        if (msg.Text == "Канал")
        {
            await botClient.SendTextMessageAsync(msg.Chat.Id, "Канал новостей: https://t.me/fitappplan");
            return;
        }
    }
});

app.Run();
