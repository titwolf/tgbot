using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    private static TelegramBotClient? bot;

    static async Task Main()
    {
        string? token = Environment.GetEnvironmentVariable("8206787948:AAFdOkk9Shgc-WfL8Vv9SDu7MOr0gNB7zN0");

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
        if (update.Type != UpdateType.Message || update.Message!.Type != MessageType.Text)
            return;

        var msg = update.Message;
        var text = msg.Text!.Trim();

        // Кнопки под строкой ввода
        ReplyKeyboardMarkup replyKeyboard = new(
            new[]
            {
                new KeyboardButton[] { "FAQ" },
                new KeyboardButton[] { "Поддержка" },
                new KeyboardButton[] { "Канал" }
            })
        {
            ResizeKeyboard = true
        };

        // Главное – команда /start
        if (text == "/start")
        {
            // Кнопка меню (WebApp) слева вместо команды меню
            var webAppKeyboard = new ReplyKeyboardMarkup(
                new[]
                {
                    KeyboardButton.WithWebApp("Открыть приложение", new WebAppInfo
                    {
                        Url = "https://titwolf.github.io/fit-app/" // твой URL GitHub Pages
                    })
                }
            )
            {
                ResizeKeyboard = true,
                IsPersistent = true
            };

            await client.SendTextMessageAsync(
                chatId: msg.Chat.Id,
                text: "Добро пожаловать! 👋\n\nНажмите кнопку ниже, чтобы открыть приложение.",
                replyMarkup: webAppKeyboard
            );

            // Показываем нижние кнопки FAQ / Поддержка / Канал
            await Task.Delay(300);
            await client.SendTextMessageAsync(
                chatId: msg.Chat.Id,
                text: "Выберите действие 👇",
                replyMarkup: replyKeyboard
            );

            return;
        }

        // Обработка кнопок FAQ / Поддержка / Канал
        switch (text.ToLower())
        {
            case "faq":
                await client.SendTextMessageAsync(msg.Chat.Id,
                    "FitPlan — приложение для составления и отслеживания тренировок.\n" +
                    "Вы можете создавать свои программы, отслеживать дни тренировок и прогресс.");
                break;

            case "поддержка":
                await client.SendTextMessageAsync(msg.Chat.Id,
                    "Чат поддержки: @fapSupport");
                break;

            case "канал":
                await client.SendTextMessageAsync(msg.Chat.Id,
                    "Канал новостей: https://t.me/fitappplan");
                break;
        }
    }

    private static Task ErrorHandler(ITelegramBotClient client, Exception ex, CancellationToken ct)
    {
        Console.WriteLine("Ошибка: " + ex.Message);
        return Task.CompletedTask;
    }
}
