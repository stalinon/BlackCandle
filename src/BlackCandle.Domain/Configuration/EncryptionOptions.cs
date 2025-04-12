namespace BlackCandle.Domain.Configuration;

/// <summary>
///     Настройки шифрования
/// </summary>
public sealed class EncryptionOptions
{
    /// <summary>
    ///     Ключ
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    ///     IV
    /// </summary>
    public string Iv { get; set; } = string.Empty;
}
