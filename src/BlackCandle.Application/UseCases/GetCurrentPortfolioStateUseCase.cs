using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.UseCases.Abstractions;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Exceptions;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.UseCases;

/// <summary>
///     Возвращает текущее состояние портфеля с брокерского счёта
/// </summary>
public class GetCurrentPortfolioStateUseCase : IUseCase<IReadOnlyCollection<PortfolioAsset>>
{
    private readonly IDataStorageContext _dataStorage = null!;

    /// <inheritdoc cref="GetCurrentPortfolioStateUseCase"/>
    public GetCurrentPortfolioStateUseCase(IDataStorageContext dataStorage)
    {
        _dataStorage = dataStorage;
    }

    /// <inheritdoc cref="GetCurrentPortfolioStateUseCase"/>
    protected GetCurrentPortfolioStateUseCase()
    {
    }

    /// <inheritdoc />
    public virtual async Task<OperationResult<IReadOnlyCollection<PortfolioAsset>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var portfolio = await _dataStorage.PortfolioAssets.GetAllAsync();
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
