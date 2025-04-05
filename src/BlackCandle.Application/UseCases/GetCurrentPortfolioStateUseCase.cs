using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.UseCases.Abstractions;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Exceptions;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.UseCases;

/// <summary>
///     Возвращает текущее состояние портфеля с брокерского счёта
/// </summary>
internal sealed class GetCurrentPortfolioStateUseCase(IDataStorageContext dataStorage)
    : IUseCase<IReadOnlyCollection<PortfolioAsset>>
{
    /// <inheritdoc />
    public async Task<OperationResult<IReadOnlyCollection<PortfolioAsset>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var portfolio = await dataStorage.PortfolioAssets.GetAllAsync();
            if (portfolio.Count == 0)
            {
                throw new NotFoundException(nameof(PortfolioAsset));
            }

            return OperationResult<IReadOnlyCollection<PortfolioAsset>>.Success(portfolio);
        }
        catch (Exception ex)
        {
            return OperationResult<IReadOnlyCollection<PortfolioAsset>>.Failure("Ошибка при получении портфеля: " + ex.Message);
        }
    }
}
