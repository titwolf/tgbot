using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;

var builder = WebApplication.CreateBuilder(args);

// переменные окружения для Render
string botToken = Environment.GetEnvironmentVariable("8206787948:AAFdOkk9Shgc-WfL8Vv9SDu7MOr0gNB7zN0");
string appUrl = Environment.GetEnvironmentVariable("https://titwolf.github.io/webapp/");

var botClient = new TelegramBotClient(botToken);

builder.Services.AddSingleton(botClient);

var app = builder.Build();

app.MapPost($"/bot{botToken}", async (Update update, ITelegramBotClient botClient) =>
{
    try
    {
        if (update.Type == UpdateType.Message && update.Message!.Text != null)
        {
            await HandleMessage(update.Message, botClient);
        }
        else if (update.Type == UpdateType.CallbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
        }
    }
    catch
    {
        // ignore
    }

    return Results.Ok();
});

// --- Установка Webhook ---
app.Lifetime.ApplicationStarted.Register(async () =>
{
    await botClient.SetWebhook($"{appUrl}/bot{botToken}");
});

app.Run();

// ------------------ HANDLERS -------------------

async Task HandleMessage(Message msg, ITelegramBotClient bot)
{
    var chatId = msg.Chat.Id;

    // Кнопка мини-приложения (вместо кнопки меню)
    ReplyKeyboardMarkup menuButton = new(new[]
    {
        new KeyboardButton[]
        {
            new KeyboardButton("Открыть приложение")
            {
                WebApp = new WebAppInfo()
                {
                    Url = "https://твоя-ссылка-на-приложение" // <-- ВСТАВЬ СЮДА URL твоего мини-приложения
                }
            }
        }
    })
    {
        ResizeKeyboard = true,
        OneTimeKeyboard = false,
    };

    // Кнопки под строкой ввода
    var bottomButtons = new ReplyKeyboardMarkup(new[]
    {
        new KeyboardButton[] { "FAQ", "Поддержка", "Канал" }
    })
    {
        ResizeKeyboard = true
    };

    string text = msg.Text.ToLower();

    if (text == "/start")
    {
        await bot.SendMessage(chatId,
            "Добро пожаловать! Открой мини-приложение или выбери кнопку ниже:",
            replyMarkup: menuButton);

        await bot.SendMessage(chatId,
            "Дополнительные кнопки:",
            replyMarkup: bottomButtons);
        return;
    }

    switch (text)
    {
        case "faq":
            await bot.SendMessage(chatId,
                "📌 *FitPlan — это приложение для составления и ведения тренировки.*\n\n" +
                "Ты можешь:\n" +
                "• Создавать свои тренировки\n" +
                "• Вести учёт занятий\n" +
                "• Следить за прогрессом\n" +
                "• Всё бесплатно и просто",
                parseMode: ParseMode.Markdown);
            break;

        case "поддержка":
            await bot.SendMessage(chatId, "Чат поддержки: @fapSupport");
            break;

        case "канал":
            await bot.SendMessage(chatId, "Канал новостей: https://t.me/fitappplan");
            break;

        default:
            await bot.SendMessage(chatId, "Выберите кнопку на панели снизу.");
            break;
    }
}
