using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.ValueObjects;

using Microsoft.AspNetCore.Mvc;

namespace BlackCandle.API.Controllers;

/// <summary>
///     Контроллер для управления накстройками бота
/// </summary>
[Route("api/settings")]
public sealed class SettingsController(IBotSettingsService service) : BaseController
{
    /// <summary>
    ///     Получить настройки
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(OperationResult<BotSettings>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<BotSettings>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperationResult<BotSettings>>> Get()
    {
        try
        {
            var settings = await service.GetAsync();
            return Success(settings);
        }
        catch (Exception e)
        {
            return Fail<BotSettings>(e.Message);
        }
    }

    /// <summary>
    ///     Сохранить настройки
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OperationResult<BotSettings>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<BotSettings>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Save(BotSettings settings)
    {
        await service.SaveAsync(settings);
        return Ok();
    }
}
