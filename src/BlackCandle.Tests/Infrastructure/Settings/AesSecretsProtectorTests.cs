using System.Security.Cryptography;

using BlackCandle.Domain.Configuration;
using BlackCandle.Infrastructure.Settings;

using FluentAssertions;

using Microsoft.Extensions.Options;

namespace BlackCandle.Tests.Infrastructure.Settings;

/// <summary>
///     Тесты на <see cref="AesSecretsProtector" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Шифрует и расшифровывает строку корректно</item>
///         <item>Возвращает вход без изменений, если строка пустая или null</item>
///     </list>
/// </remarks>
public sealed class AesSecretsProtectorTests
{
    private readonly AesSecretsProtector _protector;

    /// <inheritdoc cref="AesSecretsProtectorTests" />
    public AesSecretsProtectorTests()
    {
        var key = RandomNumberGenerator.GetBytes(32); // 256-bit
        var iv = RandomNumberGenerator.GetBytes(16);  // 128-bit

        var options = Options.Create(new EncryptionOptions
        {
            Key = Convert.ToBase64String(key),
            Iv = Convert.ToBase64String(iv),
        });

        _protector = new AesSecretsProtector(options);
    }

    /// <summary>
    ///     Тест 1: Шифрует и расшифровывает строку корректно
    /// </summary>
    [Fact(DisplayName = "Тест 1: Шифрует и расшифровывает строку корректно")]
    public void EncryptDecrypt_ShouldBeSymmetric()
    {
        // Arrange
        const string original = "MySecret123_!@#";

        // Act
        var encrypted = _protector.Encrypt(original);
        var decrypted = _protector.Decrypt(encrypted);

        // Assert
        encrypted.Should().NotBeNullOrWhiteSpace().And.NotBe(original);
        decrypted.Should().Be(original);
    }

    /// <summary>
    ///     Тест 2: Encrypt возвращает вход, если строка пустая
    /// </summary>
    [Theory(DisplayName = "Тест 2: Encrypt возвращает вход, если строка пустая")]
    [InlineData("")]
    [InlineData("   ")]
    public void Encrypt_ShouldReturnOriginal_WhenInputIsNullOrEmpty(string input)
    {
        var encrypted = _protector.Encrypt(input);
        encrypted.Should().Be(input);
    }

    /// <summary>
    ///     Тест 3: Decrypt возвращает вход, если строка пустая
    /// </summary>
    [Theory(DisplayName = "Тест 3: Decrypt возвращает вход, если строка пустая")]
    [InlineData("")]
    [InlineData("   ")]
    public void Decrypt_ShouldReturnOriginal_WhenInputIsNullOrEmpty(string input)
    {
        var decrypted = _protector.Decrypt(input);
        decrypted.Should().Be(input);
    }
}
