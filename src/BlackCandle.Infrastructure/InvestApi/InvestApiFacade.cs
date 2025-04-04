using BlackCandle.Application.Interfaces.InvestApi;

namespace BlackCandle.Infrastructure.InvestApi;

/// <inheritdoc cref="IInvestApiFacade" />
internal sealed class InvestApiFacade : IInvestApiFacade
{
    /// <inheritdoc />
    public IMarketDataClient Marketdata { get; }
    
    /// <inheritdoc />
    public IPortfolioClient Portfolio { get; }
    
    /// <inheritdoc />
    public ITradingClient Trading { get; }
    
    /// <inheritdoc />
    public IFundamentalDataClient Fundamentals { get; }

    /// <inheritdoc cref="InvestApiFacade" />
    public InvestApiFacade(
        IMarketDataClient marketdata,
        IPortfolioClient portfolio,
        ITradingClient trading,
        IFundamentalDataClient fundamentals)
    {
        Marketdata = marketdata;
        Portfolio = portfolio;
        Trading = trading;
        Fundamentals = fundamentals;
    }
}