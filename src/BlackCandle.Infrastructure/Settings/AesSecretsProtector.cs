using System.Security.Cryptography;
using System.Text;

using BlackCandle.Domain.Configuration;
using BlackCandle.Domain.Interfaces;

using Microsoft.Extensions.Options;

namespace BlackCandle.Infrastructure.Settings;

/// <summary>
///     Протектор, основанный на AES
/// </summary>
internal sealed class AesSecretsProtector : ISecretsProtector
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    /// <inheritdoc cref="AesSecretsProtector" />
    public AesSecretsProtector(IOptions<EncryptionOptions> options)
    {
        _key = Convert.FromBase64String(options.Value.Key);
        _iv = Convert.FromBase64String(options.Value.Iv);
    }

    /// <summary>
    ///     Зашифровать
    /// </summary>
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText))
        {
            return plainText;
        }

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var input = Encoding.UTF8.GetBytes(plainText);

        var encrypted = encryptor.TransformFinalBlock(input, 0, input.Length);
        return Convert.ToBase64String(encrypted);
    }

    /// <summary>
    ///     Расшифровать
    /// </summary>
    public string Decrypt(string encryptedText)
    {
        if (string.IsNullOrWhiteSpace(encryptedText))
        {
            return encryptedText;
        }

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        var input = Convert.FromBase64String(encryptedText);

        var decrypted = decryptor.TransformFinalBlock(input, 0, input.Length);
        return Encoding.UTF8.GetString(decrypted);
    }
}
