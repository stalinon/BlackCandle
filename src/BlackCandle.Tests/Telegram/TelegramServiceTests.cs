using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Configuration;
using BlackCandle.Domain.Entities;
using BlackCandle.Telegram;

using Moq;

using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BlackCandle.Tests.Telegram;

/// <summary>
///     Тесты на <see cref="TelegramService" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Отправляет сообщение</item>
///         <item>Отправляет файл</item>
///         <item>Логирует ошибку при отправке текста</item>
///         <item>Логирует ошибку при отправке файла</item>
///     </list>
/// </remarks>
public sealed class TelegramServiceTests
{
    private const string ChatId = "1337";

    private readonly Mock<IBotSettingsService> _botSettingsMock = new();
    private readonly Mock<ILoggerService> _loggerMock = new();
    private readonly Mock<ITelegramBotClient> _botMock = new();

    private readonly TelegramService _service;

    /// <inheritdoc cref="TelegramServiceTests"/>
    public TelegramServiceTests()
    {
        _botSettingsMock.Setup(x => x.GetAsync()).ReturnsAsync(new BotSettings
        {
            TelegramOptions = new TelegramOptions
            {
                BotToken = "dummy-token",
                ChatId = ChatId,
            },
        });

        _botMock.Setup(x => x.SendRequest(It.IsAny<SendMessageRequest>(), default))
            .ReturnsAsync(new Message());

        _botMock.Setup(x => x.SendRequest(It.IsAny<SendDocumentRequest>(), default))
            .ReturnsAsync(new Message());

        _service = new TelegramServiceProxy(_botSettingsMock.Object, _loggerMock.Object, _botMock.Object);
    }

    /// <summary>
    ///     Тест 1: Отправка текста вызывает Telegram API
    /// </summary>
    [Fact(DisplayName = "Тест 1: Отправка текста вызывает Telegram API")]
    public async Task SendMessageAsync_ShouldCallTelegramApi()
    {
        // Act
        await _service.SendMessageAsync("hello", disableNotification: true);

        // Assert
        _botMock.Verify(
            x => x.SendRequest(
            It.Is<SendMessageRequest>(r =>
                r.Text == "hello" &&
                r.ChatId == ChatId &&
                r.ParseMode == ParseMode.Markdown &&
                r.DisableNotification == true), default),
            Times.Once);
    }

    /// <summary>
    ///     Тест 2: Отправка файла вызывает Telegram API
    /// </summary>
    [Fact(DisplayName = "Тест 2: Отправка файла вызывает Telegram API")]
    public async Task SendFileAsync_ShouldCallTelegramApi()
    {
        using var stream = new MemoryStream("file content"u8.ToArray());

        // Act
        await _service.SendFileAsync(stream, "file.pdf", "caption here");

        // Assert
        _botMock.Verify(
            x => x.SendRequest(
            It.Is<SendDocumentRequest>(r =>
                r.Caption == "caption here" &&
                r.ChatId == ChatId &&
                r.ParseMode == ParseMode.Markdown &&
                r.Document is InputFileStream), default),
            Times.Once);
    }

    /// <summary>
    ///     Тест 3: Ошибка при отправке сообщения логируется
    /// </summary>
    [Fact(DisplayName = "Тест 3: Ошибка при отправке сообщения логируется")]
    public async Task SendMessageAsync_ShouldLogError_OnException()
    {
        _botMock.Setup(x => x.SendRequest(It.IsAny<SendMessageRequest>(), default))
            .ThrowsAsync(new Exception("fail"));

        await _service.SendMessageAsync("err");

        _loggerMock.Verify(
            x => x.LogError("Ошибка при отправке Telegram-сообщения", It.Is<Exception>(e => e.Message == "fail")),
            Times.Once);
    }

    /// <summary>
    ///     Тест 4: Ошибка при отправке файла логируется
    /// </summary>
    [Fact(DisplayName = "Тест 4: Ошибка при отправке файла логируется")]
    public async Task SendFileAsync_ShouldLogError_OnException()
    {
        using var stream = new MemoryStream("капут"u8.ToArray());

        _botMock.Setup(x => x.SendRequest(It.IsAny<SendDocumentRequest>(), default))
            .ThrowsAsync(new Exception("file fail"));

        await _service.SendFileAsync(stream, "fail.pdf", "💀");

        _loggerMock.Verify(
            x => x.LogError("Ошибка при отправке файла в Telegram", It.Is<Exception>(e => e.Message == "file fail")),
            Times.Once);
    }

    /// <summary>
    ///     Тест 5: При длинном сообщении вызывается SendFileAsync
    /// </summary>
    [Fact(DisplayName = "Тест 5: При длинном сообщении вызывается SendFileAsync")]
    public async Task SendMessageAsync_ShouldSendFile_WhenMessageTooLong()
    {
        // arrange
        var longMessage = new string('A', 5000); // больше лимита

        var fileWasSent = false;

        // подменяем TelegramService с перехватом SendFileAsync
        var service = new TelegramServiceProxy(_botSettingsMock.Object, _loggerMock.Object, _botMock.Object,
            onSendFile: () => fileWasSent = true);

        // act
        await service.SendMessageAsync(longMessage);

        // assert
        Assert.True(fileWasSent);
    }

    /// <summary>
    ///     Тест 6: Ошибка в SendFileAsync логируется
    /// </summary>
    [Fact(DisplayName = "Тест 6: Ошибка в SendFileAsync логируется")]
    public async Task SendMessageAsync_ShouldLogError_WhenSendFileFails()
    {
        var longMessage = new string('B', 5000); // снова длинное

        var service = new TelegramServiceProxy(_botSettingsMock.Object, _loggerMock.Object, _botMock.Object,
            onSendFile: () => throw new Exception("stream exploded"));

        await service.SendMessageAsync(longMessage);

        _loggerMock.Verify(
            x => x.LogError(
                "Ошибка при отправке Telegram-сообщения",
                It.Is<Exception>(e => e.Message == "stream exploded")),
            Times.Once);
    }

    /// <summary>
    ///     Прокси-реализация для внедрения мокнутого TelegramBotClient
    /// </summary>
    private sealed class TelegramServiceProxy : TelegramService
    {
        private readonly ITelegramBotClient _mockBot;
        private readonly Action? _onSendFile;
        private readonly string _chatId;

        /// <inheritdoc cref="TelegramServiceProxy" />
        public TelegramServiceProxy(
            IBotSettingsService settingsService,
            ILoggerService logger,
            ITelegramBotClient bot,
            Action? onSendFile = null)
            : base(settingsService, logger)
        {
            _mockBot = bot;
            _onSendFile = onSendFile;
            _chatId = ChatId;
        }

        /// <inheritdoc />
        public override async Task SendFileAsync(Stream fileStream, string fileName, string caption = "")
        {
            if (_onSendFile != null)
            {
                _onSendFile?.Invoke();
            }
            else
            {
                await base.SendFileAsync(fileStream, fileName, caption);
            }

            await Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override Task<(ITelegramBotClient Bot, string ChatId)> GetBotSettings()
            => Task.FromResult((_mockBot, _chatId));
    }
}
