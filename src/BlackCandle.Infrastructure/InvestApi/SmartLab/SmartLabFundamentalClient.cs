using System.Globalization;

using AngleSharp.Dom;
using AngleSharp.Html.Parser;

using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Exceptions;

namespace BlackCandle.Infrastructure.InvestApi.SmartLab;

/// <summary>
///     Скрапер таблицы фундаментальных данных с Smart-Lab
/// </summary>
/// <inheritdoc cref="SmartLabFundamentalClient" />
internal class SmartLabFundamentalClient(IDataStorageContext context, ILoggerService logger) : IFundamentalDataClient
{
    /// <summary>
    ///     Url к таблице
    /// </summary>
    protected Uri tableUri = new("https://smart-lab.ru/q/shares_fundamental/");

    private readonly HttpClient _httpClient = new();
    private readonly IRepository<FundamentalData> _repository = context.Fundamentals;

    /// <inheritdoc />
    public async Task<FundamentalData?> GetFundamentalsAsync(Ticker ticker)
    {
        var existing = await _repository.GetByIdAsync(ticker.Symbol);
        if (existing != null && DateTime.UtcNow.Date == existing.LastUpdated.Date)
        {
            logger.LogInfo($"SmartLab: возвращаем кэшированные фундаментальные данные по {ticker.Symbol}");
            return existing;
        }

        logger.LogInfo("SmartLab: загрузка таблицы фундаментала с сайта");
        var table = await LoadFundamentalsTableAsync();

        foreach (var entry in table.Values)
        {
            entry.LastUpdated = DateTime.UtcNow;
            await _repository.AddAsync(entry); // overwrite по ID
        }

        return table.GetValueOrDefault(ticker.Symbol.ToUpper());
    }

    /// <summary>
    ///     Парсим значения из таблицы
    /// </summary>
    internal static decimal? ParseDecimal(string input)
    {
        input = input.Replace("%", string.Empty).Replace("−", "-").Replace(" ", string.Empty).Trim();

        return decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
            ? result
            : null;
    }

    /// <summary>
    ///     Метод для парсинга страницы с фундаментальными данными
    /// </summary>
    protected Task<Dictionary<string, FundamentalData>> ParseTableAsync(IDocument doc)
    {
        var table = doc.QuerySelector("table[class*='simple-little-table']");
        var rows = table?.QuerySelectorAll("tr").Skip(1); // первая строка — заголовки
        if (rows == null)
        {
            return Task.FromResult<Dictionary<string, FundamentalData>>([]);
        }

        var fundamentals = new Dictionary<string, FundamentalData>();

        foreach (var row in rows)
        {
            var cells = row.QuerySelectorAll("td").ToList();
            var symbol = cells[2].TextContent.Trim().ToUpper();

            var data = new FundamentalData
            {
                Ticker = symbol,
                PERatio = ParseDecimal(cells[12].TextContent),
                PBRatio = ParseDecimal(cells[14].TextContent),
                DividendYield = ParseDecimal(cells[9].TextContent),
                MarketCap = ParseDecimal(cells[5].TextContent),
                ROE = ParseDecimal(cells[16].TextContent) / 100,
                LastUpdated = DateTime.UtcNow,
            };

            fundamentals[symbol] = data;
        }

        return Task.FromResult(fundamentals);
    }

    /// <summary>
    ///     Скрапинг таблицы
    /// </summary>
    protected virtual async Task<Dictionary<string, FundamentalData>> LoadFundamentalsTableAsync()
    {
        try
        {
            var html = await _httpClient.GetStringAsync(tableUri);
            var parser = new HtmlParser();
            var doc = await parser.ParseDocumentAsync(html);

            return await ParseTableAsync(doc);
        }
        catch (Exception ex)
        {
            logger.LogError("SmartLab: ошибка парсинга фундаментала", ex);
            throw new SmartLabScrapingException("SmartLab: ошибка загрузки фундаментальных данных");
        }
    }
}
