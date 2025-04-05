using BlackCandle.Application.Interfaces.InvestApi;

namespace BlackCandle.Infrastructure.InvestApi;

/// <inheritdoc cref="IInvestApiFacade" />
/// <inheritdoc cref="InvestApiFacade" />
internal sealed class InvestApiFacade(
    IMarketDataClient marketdata,
    IPortfolioClient portfolio,
    ITradingClient trading,
    IFundamentalDataClient fundamentals) : IInvestApiFacade
{
    /// <inheritdoc />
    public IMarketDataClient Marketdata { get; } = marketdata;

    /// <inheritdoc />
    public IPortfolioClient Portfolio { get; } = portfolio;

    /// <inheritdoc />
    public ITradingClient Trading { get; } = trading;

    /// <inheritdoc />
    public IFundamentalDataClient Fundamentals { get; } = fundamentals;
}
