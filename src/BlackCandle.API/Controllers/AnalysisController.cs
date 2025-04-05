using BlackCandle.Application.UseCases.Abstractions;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.ValueObjects;

using Microsoft.AspNetCore.Mvc;

namespace BlackCandle.API.Controllers;

/// <summary>
///     Контроллер для анализа
/// </summary>
[Route("api/analysis")]
public sealed class AnalysisController(
    IUseCase<IReadOnlyCollection<TradeSignal>> getSignals,
    IUseCase<string> runAnalysis)
    : BaseController
{
    /// <summary>
    ///     Получить сигналы последнего анализа
    /// </summary>
    [HttpGet("signals")]
    [ProducesResponseType(typeof(OperationResult<IReadOnlyCollection<TradeSignal>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<IReadOnlyCollection<TradeSignal>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSignals(CancellationToken ct)
    {
        var result = await getSignals.ExecuteAsync(ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    ///     Запустить анализ портфеля
    /// </summary>
    [HttpPost("run")]
    [ProducesResponseType(typeof(OperationResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RunAnalysis(CancellationToken ct)
    {
        var result = await runAnalysis.ExecuteAsync(ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
