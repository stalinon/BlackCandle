namespace BlackCandle.Domain.Exceptions;

/// <summary>
///     Исключение, вызванное ошибкой при скрапинге Smart Lab
/// </summary>
public sealed class SmartLabScrapingException : BlackCandleException
{
    /// <inheritdoc cref="SmartLabScrapingException" />
    public SmartLabScrapingException() : base("Ошибка при скрапинге Smart Lab")
    { }
    
    /// <inheritdoc cref="SmartLabScrapingException" />
    public SmartLabScrapingException(string message) : base(message)
    { }
}