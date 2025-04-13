namespace BlackCandle.Web.Interfaces;

/// <summary>
///     Сервис аутентификации
/// </summary>
internal interface IAuthenticationService
{
    /// <summary>
    ///     Войти
    /// </summary>
    Task<bool> LoginAsync(string login, string password);

    /// <summary>
    ///     Выйти
    /// </summary>
    Task LogoutAsync();
}
