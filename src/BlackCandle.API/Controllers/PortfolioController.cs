using BlackCandle.API.DTO;
using BlackCandle.Application.Interfaces;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace BlackCandle.API.Controllers;

/// <summary>
///     Контроллер портфолио
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PortfolioController : BaseController
{
    private readonly IPortfolioService _portfolioService;

    /// <inheritdoc cref="PortfolioController" />
    public PortfolioController(IPortfolioService portfolioService)
    {
        _portfolioService = portfolioService;
    }

    /// <summary>
    /// Получает текущий состав портфеля.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<OperationResult<List<PortfolioViewModel>>>> Get()
    {
        var assets = await _portfolioService.GetCurrentPortfolioAsync();

        var result = assets.Select(a => new PortfolioViewModel
        {
            Symbol = a.Ticker.Symbol,
            Quantity = a.Quantity,
            PurchasePrice = a.PurchasePrice,
            PurchaseDate = a.PurchaseDate
        }).ToList();

        return Success(result);
    }

    /// <summary>
    ///     Добавляет новый актив в портфель.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OperationResult<string>>> Post([FromBody] AddTickerRequest request)
    {
        var asset = new PortfolioAsset
        {
            Ticker = new Ticker
            {
                Symbol = request.Symbol
            },
            Quantity = request.Quantity,
            PurchasePrice = request.PurchasePrice,
            PurchaseDate = request.PurchaseDate
        };

        await _portfolioService.AddAssetAsync(asset);
        return Success("Актив добавлен.");
    }

    /// <summary>
    ///     Удаляет актив из портфеля по тикеру.
    /// </summary>
    [HttpDelete("{symbol}")]
    public async Task<ActionResult<OperationResult<string>>> Delete(string symbol)
    {
        await _portfolioService.RemoveAssetAsync(symbol);
        return Success("Актив удалён.");
    }
}