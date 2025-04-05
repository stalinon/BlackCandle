namespace BlackCandle.Application.Interfaces.InvestApi;

/// <summary>
///     Фасад для API инвестиций
/// </summary>
public interface IInvestApiFacade
{
    /// <inheritdoc cref="IMarketDataClient" />
    public IMarketDataClient Marketdata { get; }

    /// <inheritdoc cref="IPortfolioClient" />
    public IPortfolioClient Portfolio { get; }

    /// <inheritdoc cref="ITradingClient" />
    public ITradingClient Trading { get; }

    /// <inheritdoc cref="IFundamentalDataClient" />
    public IFundamentalDataClient Fundamentals { get; }
}
