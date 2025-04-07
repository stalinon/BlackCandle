using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Infrastructure.Persistence.Redis.Entities;

/// <summary>
///     Фундаментальные данные для Redis
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal sealed class RedisFundamentalData : IStorageEntity<FundamentalData>
{
    /// <summary>
    ///     Символ тикера
    /// </summary>
    public string Ticker { get; set; } = string.Empty;

    /// <summary>
    ///     P/E
    /// </summary>
    public decimal? PERatio { get; set; }

    /// <summary>
    ///     P/B
    /// </summary>
    public decimal? PBRatio { get; set; }

    /// <summary>
    ///     Дивдоходность
    /// </summary>
    public decimal? DividendYield { get; set; }

    /// <summary>
    ///     Капитализация
    /// </summary>
    public decimal? MarketCap { get; set; }

    /// <summary>
    ///     Рентабельность капитала
    /// </summary>
    public decimal? ROE { get; set; }

    /// <summary>
    ///     Последнее обновление
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <inheritdoc/>
    public FundamentalData ToEntity() => new()
    {
        Ticker = Ticker,
        PERatio = PERatio,
        PBRatio = PBRatio,
        DividendYield = DividendYield,
        MarketCap = MarketCap,
        ROE = ROE,
        LastUpdated = LastUpdated,
    };

    /// <inheritdoc/>
    public IStorageEntity<FundamentalData> ToStorageEntity(FundamentalData entity)
    {
        return new RedisFundamentalData
        {
            Ticker = entity.Ticker,
            PERatio = entity.PERatio,
            PBRatio = entity.PBRatio,
            DividendYield = entity.DividendYield,
            MarketCap = entity.MarketCap,
            ROE = entity.ROE,
            LastUpdated = entity.LastUpdated,
        };
    }
}
