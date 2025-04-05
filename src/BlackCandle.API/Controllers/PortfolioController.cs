using BlackCandle.Application.UseCases.Abstractions;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.ValueObjects;

using Microsoft.AspNetCore.Mvc;

namespace BlackCandle.API.Controllers;

/// <summary>
///     Контроллер для портфолио
/// </summary>
[Route("api/portfolio")]
public sealed class PortfolioController(
    IUseCase<IReadOnlyCollection<PortfolioAsset>> getPortfolioUseCase)
    : BaseController
{
    /// <summary>
    ///     Получить текущее состояние портфеля
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(OperationResult<IReadOnlyCollection<PortfolioAsset>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<IReadOnlyCollection<PortfolioAsset>>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await getPortfolioUseCase.ExecuteAsync(ct);
        return result.IsSuccess
            ? Ok(result)
            : NotFound(result);
    }
}
