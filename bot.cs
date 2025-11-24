using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    static async Task Main()
    {
        var bot = new TelegramBotClient("8206787948:AAFdOkk9Shgc-WfL8Vv9SDu7MOr0gNB7zN0");

        Console.WriteLine("Запуск long polling...");
        var me = await bot.GetMeAsync();
        Console.WriteLine($"Бот запущен: @{me.Username}");

        using CancellationTokenSource cts = new();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cts.Token
        );

        // Держим процесс активным на Render
        await Task.Delay(-1);
    }

    static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
    {
        if (update.Type == UpdateType.Message && update.Message!.Text != null)
        {
            var msg = update.Message;
            var text = msg.Text;

            if (text == "/start")
            {
                await SendStartMenu(bot, msg.Chat.Id);
                return;
            }

            if (text == "FAQ")
            {
                await bot.SendTextMessageAsync(
                    msg.Chat.Id,
                    "Это приложение предназначено для создания и отслеживания ваших тренировок.\n\n" +
                    "Вы можете:\n" +
                    "- Создавать тренировки\n" +
                    "- Вести учёт дней занятий\n" +
                    "- Просматривать свои тренировки\n" +
                    "- Использовать удобный WebApp прямо в Telegram"
                );
                return;
            }

            if (text == "Поддержка")
            {
                await bot.SendTextMessageAsync(msg.Chat.Id, "Чат поддержки: @fapSupport");
                return;
            }

            if (text == "Канал")
            {
                await bot.SendTextMessageAsync(msg.Chat.Id, "Канал новостей: https://t.me/fitappplan");
                return;
            }
        }
    }

    static async Task SendStartMenu(ITelegramBotClient bot, long chatId)
    {
        // Кнопки под строкой ввода
        var replyKeyboard = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "FAQ" },
            new KeyboardButton[] { "Поддержка" },
            new KeyboardButton[] { "Канал" }
        })
        {
            ResizeKeyboard = true,
            IsPersistent = true
        };

        // Кнопка слева — WebApp на весь экран
        var webAppUrl = "https://titwolf.github.io/webapp/";
        await bot.SetChatMenuButtonAsync(
            chatId: chatId,
            menuButton: new MenuButtonWebApp
            {
                Text = "Открыть приложение",
                WebApp = new WebAppInfo { Url = webAppUrl }
            }
        );

        await bot.SendTextMessageAsync(
            chatId,
            "Добро пожаловать!\nВыберите действие ниже или откройте приложение через кнопку слева.",
            replyMarkup: replyKeyboard
        );
    }

    static Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken token)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
        return Task.CompletedTask;
    }
}
