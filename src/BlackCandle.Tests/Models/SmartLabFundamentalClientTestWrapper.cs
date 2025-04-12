using System.Reflection;

using AngleSharp.Dom;

using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Entities;
using BlackCandle.Infrastructure.InvestApi.SmartLab;

using Moq;

namespace BlackCandle.Tests.Models;

/// <summary>
///     Обертка клиента <see cref="SmartLabFundamentalClient" /> для тестирования
/// </summary>
internal class SmartLabFundamentalClientTestWrapper : SmartLabFundamentalClient
{
    /// <inheritdoc cref="SmartLabFundamentalClientTestWrapper" />
    public SmartLabFundamentalClientTestWrapper(IRepository<FundamentalData> repo, ILoggerService logger)
        : base(repo, logger)
    {
        typeof(SmartLabFundamentalClient)
            .GetField("_repository", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(this, repo);
    }

    /// <summary>
    ///     Симулировать ошибку
    /// </summary>
    public bool SimulateFailure { get; set; }

    /// <summary>
    ///     Распарсить HTML
    /// </summary>
    public async Task<Dictionary<string, FundamentalData>> TestParseHtml(IDocument doc)
    {
        return await ParseTableAsync(doc);
    }

    /// <summary>
    ///     Тестовый запуск
    /// </summary>
    public async Task<FundamentalData?> TestGetFundamentalsAsync(Ticker ticker)
    {
        return await GetFundamentalsAsync(ticker);
    }

    /// <inheritdoc />
    protected override async Task<Dictionary<string, FundamentalData>> LoadFundamentalsTableAsync()
    {
        if (SimulateFailure)
        {
            tableUri = new Uri("https://smartlab.org/qwerqwrqewewrewrew");
        }

        return await base.LoadFundamentalsTableAsync();
    }
}
