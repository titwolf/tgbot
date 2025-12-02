using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

[ApiController]
[Route("bot-webhook")]
public class BotController : ControllerBase
{
    private readonly BotService _botService;

    public BotController(BotService botService)
    {
        _botService = botService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update)
    {
        // Быстрое Ok для пустых обновлений
        if (update == null) return Ok();

        // Обрабатываем асинхронно
        await _botService.HandleUpdateAsync(update);
        return Ok();
    }

    // Telegram никогда не делает GET на webhook — 405 нормален, но для диагностики можно оставить:
    [HttpGet]
    public IActionResult Get() => Ok("Webhook endpoint (POST only).");
}
