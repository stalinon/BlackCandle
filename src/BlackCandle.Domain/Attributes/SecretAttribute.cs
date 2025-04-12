namespace BlackCandle.Domain.Attributes;

/// <summary>
///     Пометка для чувствительных полей
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class SecretAttribute : Attribute;
