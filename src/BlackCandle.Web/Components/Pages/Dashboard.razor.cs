using BlackCandle.Application.Interfaces.Pipelines;
using BlackCandle.Application.Pipelines.AutoTradeExecution;
using BlackCandle.Application.Pipelines.PortfolioAnalysis;
using BlackCandle.Application.UseCases;
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
    public GetCurrentPortfolioStateUseCase GetPortfolioUseCase { get; set; } = default!;

    /// <summary>
    ///     Фабрика пайплайнов
    /// </summary>
    [Inject]
    public IPipelineFactory PipelineFactory { get; set; } = default!;

    /// <summary>
    ///     Снакбар
    /// </summary>
    [Inject]
    public ISnackbar Snackbar { get; set; } = default!;

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

        var pipeline = PipelineFactory.Create<PortfolioAnalysisPipeline, PortfolioAnalysisContext>();

        try
        {
            await pipeline.RunAsync();
            Snackbar.Add("Анализ успешно выполнен", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Ошибка анализа: {ex.Message}", Severity.Error);
        }

        IsRunningAnalysis = false;
        await ReloadPortfolio();
    }

    private async Task RunAutoTrade()
    {
        IsRunningTrade = true;
        StateHasChanged();

        var pipeline = PipelineFactory.Create<AutoTradeExecutionPipeline, AutoTradeExecutionContext>();

        try
        {
            await pipeline.RunAsync();
            Snackbar.Add("Автотрейдинг успешно завершён", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Ошибка анализа: {ex.Message}", Severity.Error);
            Snackbar.Add($"Ошибка автотрейдинга: {ex.Message}", Severity.Error);
        }

        IsRunningTrade = false;
        await ReloadPortfolio();
    }
}
