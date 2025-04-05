namespace BlackCandle.Domain.Exceptions;

/// <summary>
///     Выбрасывается, если что-то не нашлось.
/// </summary>
public class NotFoundException(string? smf = null) : BlackCandleException("Не найдено сущностей" + (smf != null ? $"типа {smf}" : string.Empty));
