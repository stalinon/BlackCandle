using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.UseCases.Abstractions;

/// <summary>
///     Use-case
/// </summary>
public interface IUseCase<in TInput, TOutput>
{
    /// <summary>
    ///     Выполнить
    /// </summary>
    Task<OperationResult<TOutput>> ExecuteAsync(TInput input, CancellationToken cancellationToken = default);
}

/// <summary>
///     Use-case
/// </summary>
public interface IUseCase<TOutput>
{
    /// <summary>
    ///     Выполнить
    /// </summary>
    Task<OperationResult<TOutput>> ExecuteAsync(CancellationToken cancellationToken = default);
}
