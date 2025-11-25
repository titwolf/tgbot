#nullable enable
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

public class BotService
{
    private readonly TelegramBotClient _bot;

    public BotService(TelegramBotClient bot)
    {
        _bot = bot;
    }

    public async Task HandleUpdate(Update update)
    {
        if (update.Message == null || update.Message.Text == null)
            return;

        var text = update.Message.Text;
        long chatId = update.Message.Chat.Id;

        // Клавиатура снизу
        var bottomButtons = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "FAQ" },
            new KeyboardButton[] { "Поддержка" },
            new KeyboardButton[] { "Канал" }
        })
        {
            ResizeKeyboard = true
        };

        if (text == "/start")
        {
            var startKeyboard = new ReplyKeyboardMarkup(
                KeyboardButton.WithWebApp(
                    "Открыть приложение",
                    new WebAppInfo
                    {
                        Url = "https://titwolf.github.io/fit-app/"
                    })
            )
            {
                ResizeKeyboard = true
            };

            await _bot.SendTextMessageAsync(chatId,
                "Добро пожаловать! Нажмите кнопку, чтобы открыть приложение.",
                replyMarkup: startKeyboard);

            await _bot.SendTextMessageAsync(chatId,
                "Выберите действие:",
                replyMarkup: bottomButtons);

            return;
        }

        // Обработка кнопок
        switch (text.ToLower())
        {
            case "faq":
                await _bot.SendTextMessageAsync(chatId,
                    "FitPlan — приложение для составления и отслеживания тренировок.");
                break;

            case "поддержка":
                await _bot.SendTextMessageAsync(chatId, "Чат поддержки: @fapSupport");
                break;

            case "канал":
                await _bot.SendTextMessageAsync(chatId, "Канал: https://t.me/fitappplan");
                break;
        }
    }
}
