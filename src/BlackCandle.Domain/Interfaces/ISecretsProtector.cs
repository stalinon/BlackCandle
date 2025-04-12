namespace BlackCandle.Domain.Interfaces;

/// <summary>
///     Сервис шифрования
/// </summary>
public interface ISecretsProtector
{
    /// <summary>
    ///     Зашифровать
    /// </summary>
    string Encrypt(string plainText);

    /// <summary>
    ///     Расшифровать
    /// </summary>
    string Decrypt(string encryptedText);
}
