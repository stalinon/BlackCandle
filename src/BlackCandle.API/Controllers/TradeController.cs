using BlackCandle.Application.UseCases;
using BlackCandle.Domain.ValueObjects;

using Microsoft.AspNetCore.Mvc;

namespace BlackCandle.API.Controllers;

/// <summary>
///     Контроллер для трейдов
/// </summary>
[Route("api/trade")]
public sealed class TradeController(
    PreviewTradeExecutionUseCase previewUseCase,
    RunAutoTradeExecutionUseCase runTradeUseCase)
    : BaseController
{
    /// <summary>
    ///     Предпросмотр исполнения сделок по текущим сигналам
    /// </summary>
    [HttpPost("preview")]
    [ProducesResponseType(typeof(OperationResult<IReadOnlyCollection<OrderPreview>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<IReadOnlyCollection<OrderPreview>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Preview(CancellationToken ct)
    {
        var result = await previewUseCase.ExecuteAsync(ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    ///     Запустить автоматическую торговлю
    /// </summary>
    [HttpPost("run")]
    [ProducesResponseType(typeof(OperationResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RunTrade(CancellationToken ct)
    {
        var result = await runTradeUseCase.ExecuteAsync(ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
