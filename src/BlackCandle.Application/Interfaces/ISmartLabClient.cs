namespace BlackCandle.Application.Interfaces;

/// <summary>
///     Скрейпер для получения фундаментальных данных с Smart-Lab.
/// </summary>
public interface ISmartLabClient
{
    /// <summary>
    ///     Получить фундаментальные данные
    /// </summary>
    Task<Dictionary<string, string>> GetFundamentalsAsync(string ticker);
}