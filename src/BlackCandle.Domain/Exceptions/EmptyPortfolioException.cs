namespace BlackCandle.Domain.Exceptions;

/// <summary>
///     Исключение при пустом портфеле
/// </summary>
public class EmptyPortfolioException : Exception
{
    /// <inheritdoc cref="EmptyPortfolioException" />
    public EmptyPortfolioException() : base("Портфель пуст. Анализ невозможен.") { }
}