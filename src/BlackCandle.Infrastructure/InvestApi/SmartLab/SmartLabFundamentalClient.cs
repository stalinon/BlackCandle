using System.Globalization;
using AngleSharp.Html.Parser;
using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Exceptions;

namespace BlackCandle.Infrastructure.InvestApi.SmartLab;

/// <summary>
///     Скрапер таблицы фундаментальных данных с Smart-Lab
/// </summary>
internal sealed class SmartLabFundamentalClient : IFundamentalDataClient
{
    private readonly ILoggerService _logger;
    private readonly HttpClient _httpClient;
    private readonly IRepository<FundamentalData> _repository;
    private static readonly Uri TableUri = new("https://smart-lab.ru/q/shares_fundamental/");

    /// <inheritdoc cref="SmartLabFundamentalClient" />
    public SmartLabFundamentalClient(IDataStorageContext context, ILoggerService logger)
    {
        _logger = logger;
        _httpClient = new HttpClient();
        _repository = context.Fundamentals;
    }

    /// <inheritdoc />
    public async Task<FundamentalData?> GetFundamentalsAsync(Ticker ticker)
    {
        var existing = await _repository.GetByIdAsync(ticker.Symbol);
        if (existing is { } data && DateTime.UtcNow.Date == data.LastUpdated.Date)
        {
            _logger.LogInfo($"SmartLab: возвращаем кэшированные фундаментальные данные по {ticker.Symbol}");
            return data;
        }

        _logger.LogInfo("SmartLab: загрузка таблицы фундаментала с сайта");
        var table = await LoadFundamentalsTableAsync();

        foreach (var entry in table.Values)
        {
            entry.LastUpdated = DateTime.UtcNow;
            await _repository.AddAsync(entry); // overwrite по ID
        }

        return table.GetValueOrDefault(ticker.Symbol.ToUpper());
    }

    private async Task<Dictionary<string, FundamentalData>> LoadFundamentalsTableAsync()
    {
        try
        {
            var html = await _httpClient.GetStringAsync(TableUri);
            var parser = new HtmlParser();
            var doc = await parser.ParseDocumentAsync(html);

            var table = doc.QuerySelector("table[class*='simple-little-table']");
            var rows = table?.QuerySelectorAll("tr").Skip(1); // первая строка — заголовки
            if (rows == null)
            {
                return new();
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
                    ROE = ParseDecimal(cells[16].TextContent),
                    LastUpdated = DateTime.UtcNow
                };

                fundamentals[symbol] = data;
            }

            return fundamentals;
        }
        catch (Exception ex)
        {
            _logger.LogError("SmartLab: ошибка парсинга фундаментала", ex);
            throw new SmartLabScrapingException("SmartLab: ошибка загрузки фундаментальных данных");
        }
    }

    private static decimal? ParseDecimal(string input)
    {
        input = input.Replace("%", "").Replace("−", "-").Replace(" ", "").Trim();

        return decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
            ? result
            : null;
    }
}
