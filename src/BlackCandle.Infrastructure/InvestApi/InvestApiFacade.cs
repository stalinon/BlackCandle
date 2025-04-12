using BlackCandle.Application.Interfaces.InvestApi;

namespace BlackCandle.Infrastructure.InvestApi;

/// <inheritdoc cref="IInvestApiFacade" />
internal sealed class InvestApiFacade(
    IMarketDataClient marketdata,
    IPortfolioClient portfolio,
    ITradingClient trading,
    IFundamentalDataClient fundamentals,
    IInstrumentClient instruments) : IInvestApiFacade
{
    /// <inheritdoc />
    public IMarketDataClient Marketdata { get; } = marketdata;

    /// <inheritdoc />
    public IPortfolioClient Portfolio { get; } = portfolio;

    /// <inheritdoc />
    public ITradingClient Trading { get; } = trading;

    /// <inheritdoc />
    public IFundamentalDataClient Fundamentals { get; } = fundamentals;

    /// <inheritdoc />
    public IInstrumentClient Instruments { get; } = instruments;
}
