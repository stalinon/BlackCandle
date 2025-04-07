using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Infrastructure.Persistence.Redis.Entities;

/// <summary>
///     Инструмент в портфеле для Redis
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal sealed class RedisPortfolioAsset : IStorageEntity<PortfolioAsset>
{
    /// <summary>
    ///     Тикер бумаги
    /// </summary>
    public Ticker Ticker { get; set; } = new();

    /// <summary>
    ///     Количество
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    ///     Цена
    /// </summary>
    public decimal CurrentValue { get; set; }

    /// <inheritdoc />
    public PortfolioAsset ToEntity() => new()
    {
        Ticker = Ticker,
        Quantity = Quantity,
        CurrentValue = CurrentValue,
    };

    /// <inheritdoc />
    public IStorageEntity<PortfolioAsset> ToStorageEntity(PortfolioAsset entity)
    {
        return new RedisPortfolioAsset
        {
            Ticker = entity.Ticker,
            Quantity = entity.Quantity,
            CurrentValue = entity.CurrentValue,
        };
    }
}
