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
            }
        }
    }

    static async Task SendStartMenu(ITelegramBotClient bot, long chatId)
    {
        // WebApp-кнопка вместо меню ⋮
        var webAppKeyboard = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[]
            {
                new KeyboardButton("Открыть приложение")
                {
                    WebApp = new WebAppInfo
                    {
                        Url = "https://titwolf.github.io/webapp/"
                    }
                }
            }
        })
        {
            ResizeKeyboard = true,
            IsPersistent = true
        };

        // Кнопки команд под полем ввода
        var commandKeyboard = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { new KeyboardButton("/support") },
            new KeyboardButton[] { new KeyboardButton("/channel") },
            new KeyboardButton[] { new KeyboardButton("/faq") }
        })
        {
            ResizeKeyboard = true,
            IsPersistent = true
        };

        // Отправляем WebApp-кнопку первым сообщением
        await bot.SendTextMessageAsync(
            chatId,
            "Нажми кнопку ниже, чтобы открыть приложение:",
            replyMarkup: webAppKeyboard
        );

        // Отправляем кнопки команд отдельным сообщением
        await bot.SendTextMessageAsync(
            chatId,
            "Команды для взаимодействия с ботом:",
            replyMarkup: commandKeyboard
        );
    }

    static Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken token)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
        return Task.CompletedTask;
    }
}
