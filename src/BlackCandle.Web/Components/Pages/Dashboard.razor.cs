using BlackCandle.Application.UseCases.Abstractions;
using BlackCandle.Domain.Entities;

using Microsoft.AspNetCore.Components;

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
    ///     Активы в портфеле
    /// </summary>
    private List<PortfolioAsset> PortfolioAssets { get; set; } = [];

    /// <summary>
    ///     Признак загрузки
    /// </summary>
    private bool IsLoading { get; set; } = true;

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        var result = await GetPortfolioUseCase.ExecuteAsync();
        if (result.IsSuccess)
        {
            PortfolioAssets = result.Data?.ToList() ?? [];
        }
        else
        {
            Console.WriteLine($"Ошибка получения портфеля: {result.Error}");
        }

        IsLoading = false;
    }
}
