using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Configuration;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Exceptions;
using BlackCandle.Domain.Interfaces;
using BlackCandle.Infrastructure.Settings;

using FluentAssertions;

using Moq;

namespace BlackCandle.Tests.Infrastructure.Settings;

/// <summary>
///     Тесты на <see cref="BotSettingsService" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>GetAsync возвращает расшифрованные данные</item>
///         <item>GetAsync бросает исключение, если настроек нет</item>
///         <item>SaveAsync вызывает шифрование и сохранение</item>
///         <item>Secret-свойства вложенных объектов обрабатываются корректно</item>
///     </list>
/// </remarks>
public sealed class BotSettingsServiceTests
{
    private readonly Mock<ISecretsProtector> _protectorMock = new();
    private readonly Mock<IRepository<BotSettings>> _repoMock = new();
    private readonly Mock<IDataStorageContext> _ctxMock = new();

    private readonly BotSettingsService _service;

    /// <inheritdoc cref="BotSettingsServiceTests"/>
    public BotSettingsServiceTests()
    {
        _ctxMock.Setup(x => x.BotSettings).Returns(_repoMock.Object);

        _protectorMock
            .Setup(p => p.Encrypt(It.IsAny<string>()))
            .Returns((string s) => $"enc({s})");

        _protectorMock
            .Setup(p => p.Decrypt(It.IsAny<string>()))
            .Returns((string s) => s.Replace("enc(", string.Empty).Replace(")", string.Empty));

        _service = new BotSettingsService(_ctxMock.Object, _protectorMock.Object);
    }

    /// <summary>
    ///     Тест 1: GetAsync возвращает расшифрованные данные
    /// </summary>
    [Fact(DisplayName = "Тест 1: GetAsync возвращает расшифрованные данные")]
    public async Task GetAsync_ShouldDecryptSecrets()
    {
        // Arrange
        var settings = new BotSettings
        {
            TelegramOptions = new TelegramOptions { BotToken = "enc(token)", ChatId = "chat" },
        };

        _repoMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(settings);

        // Act
        var result = await _service.GetAsync();

        // Assert
        result.TelegramOptions!.BotToken.Should().Be("token");
        result.TelegramOptions.ChatId.Should().Be("chat");
    }

    /// <summary>
    ///     Тест 2: GetAsync бросает исключение, если настроек нет
    /// </summary>
    [Fact(DisplayName = "Тест 2: GetAsync бросает исключение, если настроек нет")]
    public async Task GetAsync_ShouldThrow_WhenSettingsMissing()
    {
        _repoMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((BotSettings?)null);

        var act = () => _service.GetAsync();

        await act.Should().ThrowAsync<BotNotConfiguredException>();
    }

    /// <summary>
    ///     Тест 3: SaveAsync вызывает шифрование и сохранение
    /// </summary>
    [Fact(DisplayName = "Тест 3: SaveAsync вызывает шифрование и сохранение")]
    public async Task SaveAsync_ShouldEncryptSecrets_AndSave()
    {
        // Arrange
        var settings = new BotSettings
        {
            TinkoffClientOptions = new TinkoffClientOptions { ApiKey = "secret", AccountId = "acc" },
        };

        // Act
        await _service.SaveAsync(settings);

        // Assert
        _repoMock.Verify(
            x => x.AddAsync(It.Is<BotSettings>(
                s => s.TinkoffClientOptions!.ApiKey == "enc(secret)" && s.TinkoffClientOptions.AccountId == "acc")), Times.Once);
    }

    /// <summary>
    ///     Тест 4: Игнорируются пустые и null значения
    /// </summary>
    [Fact(DisplayName = "Тест 4: Игнорируются пустые и null значения")]
    public async Task SaveAsync_ShouldIgnoreEmptyOrNullSecrets()
    {
        // Arrange
        var settings = new BotSettings
        {
            TelegramOptions = new TelegramOptions { BotToken = string.Empty, ChatId = null! },
        };

        // Act
        await _service.SaveAsync(settings);

        // Assert
        _protectorMock.Verify(x => x.Encrypt(It.IsAny<string>()), Times.Never);
    }

    /// <summary>
    ///     Тест 5: Обрабатываются вложенные объекты с секретами
    /// </summary>
    [Fact(DisplayName = "Тест 5: Обрабатываются вложенные объекты с секретами")]
    public async Task SaveAsync_ShouldEncryptNestedSecrets()
    {
        var settings = new BotSettings
        {
            TelegramOptions = new TelegramOptions { BotToken = "nested-secret", ChatId = "123" },
        };

        // Act
        await _service.SaveAsync(settings);

        // Assert
        _repoMock.Verify(
            x => x.AddAsync(It.Is<BotSettings>(s => s.TelegramOptions!.BotToken == "enc(nested-secret)")), Times.Once);
    }
}
