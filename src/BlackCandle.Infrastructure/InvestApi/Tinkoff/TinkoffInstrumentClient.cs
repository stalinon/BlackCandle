using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Exceptions;

using Tinkoff.InvestApi.V1;

namespace BlackCandle.Infrastructure.InvestApi.Tinkoff;

/// <summary>
///     Реализация <see cref="IInstrumentClient" /> для Tinkoff API
/// </summary>
internal sealed class TinkoffInstrumentClient(ILoggerService logger, ITinkoffInvestApiClientWrapper investApiClient) : IInstrumentClient
{
    private readonly InstrumentsService.InstrumentsServiceClient _client = investApiClient.Instruments;

    /// <inheritdoc/>
    public async Task<IEnumerable<Ticker>> GetTopTickersAsync(int count)
    {
        try
        {
            var response = await _client.SharesAsync(new InstrumentsRequest
            {
                InstrumentStatus = InstrumentStatus.Base,
            });

            var now = DateTime.UtcNow;

            var tickers = response.Instruments
                .Where(i => i is
                {
                    ApiTradeAvailableFlag: true,
                    BuyAvailableFlag: true,
                    ForQualInvestorFlag: false,
                    TradingStatus: SecurityTradingStatus.NormalTrading,
                    BlockedTcaFlag: false
                })
                .Select(i => new
                {
                    i,
                    Score = CalculateScore(i, now),
                })
                .OrderByDescending(x => x.Score)
                .Take(count)
                .Select(x => new Ticker
                {
                    Symbol = x.i.Ticker,
                    Figi = x.i.Figi,
                    Currency = x.i.Currency,
                    Sector = x.i.Sector,
                });

            return tickers;
        }
        catch (Exception ex)
        {
            logger.LogError("Ошибка при получении списка тикеров от Tinkoff", ex);
            throw new TinkoffApiException("Ошибка при получении списка тикеров от Tinkoff API");
        }
    }

    private static int CalculateScore(Share i, DateTime now)
    {
        var score = 0;

        if (i.LiquidityFlag)
        {
            score += 3;
        }

        if (i is { BuyAvailableFlag: true, ApiTradeAvailableFlag: true })
        {
            score += 2;
        }

        if (!i.ForQualInvestorFlag)
        {
            score += 1;
        }

        if (i.DivYieldFlag)
        {
            score += 1;
        }

        if (i.IssueSize > 10_000_000)
        {
            score += 1;
        }

        if (i.IpoDate.ToDateTime() < now.AddYears(-1))
        {
            score += 1;
        }

        return score;
    }
}
