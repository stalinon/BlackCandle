using System.Reflection;
using System.Text;

using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Telegram;

using Microsoft.Extensions.Options;

using Moq;

using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BlackCandle.Tests.Telegram;

/// <summary>
///     Тесты на <see cref="TelegramService"/>
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Отправка текста вызывает Telegram API через SendRequest</item>
///         <item>Ошибки при отправке логируются</item>
///         <item>Исключение при отправке сообщения логируется</item>
///         <item>Исключение при отправке файла логируется</item>
///     </list>
/// </remarks>
public sealed class TelegramServiceTests
{
    private const string Token = "123:ABC";
    private const string ChatId = "666";

    private readonly Mock<ILoggerService> _loggerMock = new();
    private readonly Mock<ITelegramBotClient> _botMock = new();
    private readonly TelegramService _service;

    /// <inheritdoc cref="TelegramServiceTests" />
    public TelegramServiceTests()
    {
        var options = Options.Create(new TelegramOptions
        {
            BotToken = Token,
            ChatId = ChatId,
        });

        // Мокаем SendRequest
        _botMock.Setup(x => x.SendRequest(It.IsAny<SendMessageRequest>(), default))
            .ReturnsAsync(new Message());

        _botMock.Setup(x => x.SendRequest(It.IsAny<SendDocumentRequest>(), default))
            .ReturnsAsync(new Message());

        // Инъекция через reflection (как раньше)
        _service = (TelegramService)Activator.CreateInstance(
            typeof(TelegramService),
            args: [options, _loggerMock.Object])!;

        var field = typeof(TelegramService).GetField("_bot", BindingFlags.NonPublic | BindingFlags.Instance)!;
        field.SetValue(_service, _botMock.Object);
    }

    /// <summary>
    ///     Тест 1: Отправка текста вызывает Telegram API через SendRequest
    /// </summary>
    [Fact(DisplayName = "Тест 1: Отправка текста вызывает Telegram API через SendRequest")]
    public async Task SendMessageAsync_ShouldCallSendRequest()
    {
        // Act
        await _service.SendMessageAsync("hello", disableNotification: true);

        // Assert
        _botMock.Verify(
            x => x.SendRequest(
            It.Is<SendMessageRequest>(r =>
                r.ChatId == ChatId &&
                r.Text == "hello" &&
                r.ParseMode == ParseMode.Markdown &&
                r.DisableNotification == true),
            default), Times.Once);
    }

    /// <summary>
    ///     Тест 2: Отправка файла вызывает Telegram API через SendRequest
    /// </summary>
    [Fact(DisplayName = "Тест 2: Отправка файла вызывает Telegram API через SendRequest")]
    public async Task SendFileAsync_ShouldCallSendRequest()
    {
        // Arrange
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("file content"));

        // Act
        await _service.SendFileAsync(stream, "file.pdf", "вложение");

        // Assert
        _botMock.Verify(
            x => x.SendRequest(
            It.Is<SendDocumentRequest>(r => r.ChatId == ChatId),
            default), Times.Once);
    }

    /// <summary>
    ///     Тест 3: Исключение при отправке сообщения логируется
    /// </summary>
    [Fact(DisplayName = "Тест 3: Исключение при отправке сообщения логируется")]
    public async Task SendMessageAsync_ShouldLogErrorOnException()
    {
        // Arrange
        _botMock.Reset();
        _botMock.Setup(x => x.SendRequest(It.IsAny<SendMessageRequest>(), default))
            .ThrowsAsync(new Exception("fail"));

        var field = typeof(TelegramService).GetField("_bot", BindingFlags.NonPublic | BindingFlags.Instance)!;
        field.SetValue(_service, _botMock.Object);

        // Act
        await _service.SendMessageAsync("провал");

        // Assert
        _loggerMock.Verify(
            x => x.LogError("Ошибка при отправке Telegram-сообщения", It.Is<Exception>(e => e.Message == "fail")),
            Times.Once);
    }

    /// <summary>
    ///     Тест 4: Исключение при отправке файла логируется
    /// </summary>
    [Fact(DisplayName = "Тест 4: Исключение при отправке файла логируется")]
    public async Task SendFileAsync_ShouldLogErrorOnException()
    {
        // Arrange
        using var stream = new MemoryStream("капут"u8.ToArray());

        _botMock.Reset();
        _botMock.Setup(x => x.SendRequest(It.IsAny<SendDocumentRequest>(), default))
            .ThrowsAsync(new Exception("фейл файла"));

        var field = typeof(TelegramService).GetField("_bot", BindingFlags.NonPublic | BindingFlags.Instance)!;
        field.SetValue(_service, _botMock.Object);

        // Act
        await _service.SendFileAsync(stream, "fail.txt", "приложение");

        // Assert
        _loggerMock.Verify(
            x => x.LogError("Ошибка при отправке файла в Telegram", It.Is<Exception>(e => e.Message == "фейл файла")),
            Times.Once);
    }
}
