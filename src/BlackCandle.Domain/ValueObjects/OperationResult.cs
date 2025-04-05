namespace BlackCandle.Domain.ValueObjects;

/// <summary>
///     Стандартная обёртка ответа. Упрощает возврат с ошибкой или успехом.
/// </summary>
public class OperationResult<T>
{
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
    ///     Успех
    /// </summary>
    public static OperationResult<T> Success(T data)
    {
        return new OperationResult<T>(true, data, null);
    }

    /// <summary>
    ///     Ошибка
    /// </summary>
    public static OperationResult<T> Failure(string error)
    {
        return new OperationResult<T>(false, default, error);
    }
}
