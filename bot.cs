using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var bot = new TelegramBotClient("8206787948:AAFdOkk9Shgc-WfL8Vv9SDu7MOr0gNB7zN0");

        using CancellationTokenSource cts = new();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        bot.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandleErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        var me = await bot.GetMeAsync();
        Console.WriteLine($"Бот запущен: {me.Username}");

        // Держим бота активным 24/7
        await Task.Delay(-1, cts.Token);
    }

    // Основная логика обработки сообщений
    static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
    {
        if (update.Type == UpdateType.Message && update.Message!.Text != null)
        {
            var msg = update.Message;
            var text = msg.Text;

            switch (text)
            {
                case "/start":
                    await SendStartMenu(bot, msg.Chat.Id);
                    break;

                case "/support":
                    await bot.SendTextMessageAsync(msg.Chat.Id, "Чат поддержки: @fapSupport");
                    break;

                case "/channel":
                    await bot.SendTextMessageAsync(msg.Chat.Id, "Канал разработчика: https://t.me/fitappplan");
                    break;

                case "/faq":
                    await bot.SendTextMessageAsync(msg.Chat.Id,
                        "Это приложение предназначено для создания и отслеживания ваших тренировок.\n\n" +
                        "Вы можете:\n" +
                        "- Создавать новые тренировки\n" +
                        "- Вести учет дней занятий\n" +
                        "- Просматривать свои тренировки\n" +
                        "- Использовать удобный WebApp прямо в Telegram\n\n" +
                        "Приложение полностью бесплатное и удобно для планирования ваших тренировок."
                    );
                    break;

                case "Открыть приложение":
                    // Ничего не делаем, кнопка сама откроет WebApp
                    break;
            }
        }
    }

    // Главное меню
    static async Task SendStartMenu(ITelegramBotClient bot, long chatId)
    {
        // Кнопки команд под полем ввода
        var keyboard = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { new KeyboardButton("/support") },
            new KeyboardButton[] { new KeyboardButton("/channel") },
            new KeyboardButton[] { new KeyboardButton("/faq") }
        })
        {
            ResizeKeyboard = true,
            IsPersistent = true
        };

        // Меню ⋮ — кнопка для открытия WebApp
        await bot.SetMyCommandsAsync(new[]
        {
            new BotCommand
            {
                Command = "openapp",
                Description = "Открыть приложение"
            }
        });

        await bot.SendTextMessageAsync(
            chatId,
            "Добро пожаловать! Используй команды ниже или открой приложение через меню ⋮.",
            replyMarkup: keyboard
        );
    }

    static Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken token)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
        return Task.CompletedTask;
    }
}
