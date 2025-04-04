namespace BlackCandle.Domain.Entities;

/// <summary>
///     Тикер (инструмент)
/// </summary>
public class Ticker : IEquatable<Ticker>
{
    /// <summary>
    ///     Символ
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    ///     Валюта
    /// </summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    ///     Отрасль
    /// </summary>
    public string Sector { get; set; } = string.Empty;

    /// <summary>
    ///     Уникальный код тикера
    /// </summary>
    public string Figi { get; set; } = string.Empty;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Symbol} - {Currency} - {Sector} - {Figi}";
    }

    /// <inheritdoc />
    public bool Equals(Ticker? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Symbol == other.Symbol && Currency == other.Currency && Sector == other.Sector && Figi == other.Figi;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Ticker)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Symbol, Currency, Sector, Figi);
    }
}