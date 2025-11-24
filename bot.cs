#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    private static TelegramBotClient? bot;

    static async Task Main()
    {
        string? token = Environment.GetEnvironmentVariable("BOT_TOKEN");

        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("Ошибка: переменная окружения BOT_TOKEN не найдена.");
            return;
        }

        bot = new TelegramBotClient(token);

        Console.WriteLine("Запуск long polling...");
        var me = await bot.GetMeAsync();
        Console.WriteLine($"Бот запущен: @{me.Username}");

        bot.StartReceiving(UpdateHandler, ErrorHandler);

        await Task.Delay(-1);
    }

    private static async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken ct)
    {
        // …остальной код без изменений
    }

    private static Task ErrorHandler(ITelegramBotClient client, Exception ex, CancellationToken ct)
    {
        Console.WriteLine("Ошибка: " + ex.Message);
        return Task.CompletedTask;
    }
}
