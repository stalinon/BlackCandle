using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.Trading;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;

/// <summary>
///     Генерация сигналов на основе тех. индикаторов и фундаментала
/// </summary>
internal sealed class GenerateSignalsStep(IDataStorageContext dataStorage, ISignalGenerationStrategy signalGenerator) : PipelineStep<PortfolioAnalysisContext>
{
    /// <inheritdoc />
    public override string Name => "Генерация сигналов";

    /// <inheritdoc />
    public override async Task ExecuteAsync(
        PortfolioAnalysisContext context,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var indicatorData = context.Tickers
            .Select(t => (t, Indicators: context.Indicators.GetValueOrDefault(t)))
            .Where(a => a.Indicators is not null)
            .ToList();

        foreach (var (ticker, indicators) in indicatorData)
        {
            var score = context.FundamentalScores.GetValueOrDefault(ticker, 0);
            var signal = signalGenerator.Generate(ticker, indicators!, score, now);

            if (signal is not null)
            {
                await dataStorage.TradeSignals.AddAsync(signal);
            }
        }
    }
}
