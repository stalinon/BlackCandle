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
        : base(new Mock<IDataStorageContext>().Object, logger)
    {
        typeof(SmartLabFundamentalClient)
            .GetField("_repository", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(this, repo);
    }

    /// <summary>
    ///     Распарсить HTML
    /// </summary>
    public async Task<Dictionary<string, FundamentalData>> TestParseHtml(IDocument doc)
    {
        return await ParseTableAsync(doc);
    }
}
