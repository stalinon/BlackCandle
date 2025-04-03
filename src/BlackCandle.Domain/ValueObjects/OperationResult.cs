namespace BlackCandle.Domain.ValueObjects;

/// <summary>
///     Стандартная обёртка ответа. Упрощает возврат с ошибкой или успехом.
/// </summary>
public class OperationResult<T>
{
    /// <summary>
    ///     Индикатор успеха запроса
    /// </summary>
    public bool IsSuccess { get; }
    
    /// <summary>
    ///     Текст ошибка
    /// </summary>
    public string? Error { get; }
    
    /// <summary>
    ///     Данные
    /// </summary>
    public T? Data { get; }

    /// <summary>
    ///     Приватный конструктор
    /// </summary>
    private OperationResult(bool isSuccess, T? data, string? error)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
    }

    /// <summary>
    ///     Успех
    /// </summary>
    public static OperationResult<T> Success(T data) => new(true, data, null);
    
    /// <summary>
    ///     Ошибка
    /// </summary>
    public static OperationResult<T> Failure(string error) => new(false, default, error);
}