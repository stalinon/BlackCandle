using BlackCandle.Application.UseCases.Abstractions;
using BlackCandle.Domain.Entities;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace BlackCandle.Web.Components.Pages;

/// <summary>
///     Страница дашборда
/// </summary>
public partial class Dashboard : ComponentBase
{
    /// <summary>
    ///     Юзкейс для получения портфолио
    /// </summary>
    [Inject]
    private IUseCase<IReadOnlyCollection<PortfolioAsset>> GetPortfolioUseCase { get; set; } = default!;

    /// <summary>
    ///     Юзкейс для запуска анализа
    /// </summary>
    [Inject]
    private IUseCase<string> RunAnalysisUseCase { get; set; } = default!;

    /// <summary>
    ///     Запуск торговли
    /// </summary>
    [Inject]
    private IUseCase<string> RunTradeUseCase { get; set; } = default!;

    /// <summary>
    ///     Снакбар
    /// </summary>
    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    /// <summary>
    ///     Активы в портфеле
    /// </summary>
    private List<PortfolioAsset> PortfolioAssets { get; set; } = [];

    /// <summary>
    ///     Признак загрузки
    /// </summary>
    private bool IsLoading { get; set; } = true;

    /// <summary>
    ///     Анализ запущен
    /// </summary>
    private bool IsRunningAnalysis { get; set; }

    /// <summary>
    ///     Торговля запущена
    /// </summary>
    private bool IsRunningTrade { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await ReloadPortfolio();
    }

    private async Task ReloadPortfolio()
    {
        IsLoading = true;
        var result = await GetPortfolioUseCase.ExecuteAsync();
        if (result.IsSuccess)
        {
            PortfolioAssets = result.Data?.ToList() ?? [];
        }
        else
        {
            Snackbar.Add($"Ошибка получения портфеля: {result.Error}", Severity.Error);
        }

        IsLoading = false;
    }

    private async Task RunAnalysis()
    {
        IsRunningAnalysis = true;
        StateHasChanged();

        var result = await RunAnalysisUseCase.ExecuteAsync();

        if (result.IsSuccess)
        {
            Snackbar.Add("Анализ успешно выполнен", Severity.Success);
        }
        else
        {
            Snackbar.Add($"Ошибка анализа: {result.Error}", Severity.Error);
        }

        IsRunningAnalysis = false;
        await ReloadPortfolio();
    }

    private async Task RunAutoTrade()
    {
        IsRunningTrade = true;
        StateHasChanged();

        var result = await RunTradeUseCase.ExecuteAsync();

        if (result.IsSuccess)
        {
            Snackbar.Add("Автотрейдинг успешно завершён", Severity.Success);
        }
        else
        {
            Snackbar.Add($"Ошибка автотрейдинга: {result.Error}", Severity.Error);
        }

        IsRunningTrade = false;
        await ReloadPortfolio();
    }
}
