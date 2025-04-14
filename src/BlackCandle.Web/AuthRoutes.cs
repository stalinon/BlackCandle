namespace BlackCandle.Web;

/// <summary>
///     Роуты для авторизации и пр.
/// </summary>
public static class AuthRoutes
{
    /// <summary>
    ///     База
    /// </summary>
    public const string Base = "auth";

    /// <summary>
    ///     Логин
    /// </summary>
    public const string Login = "login";

    /// <summary>
    ///     Выход
    /// </summary>
    public const string Logout = "logout";

    /// <summary>
    ///     Схема
    /// </summary>
    public const string AuthScheme = "AdminAuthScheme";

    /// <summary>
    ///     Роль админа
    /// </summary>
    public const string AdminRole = "Admin";

    /// <summary>
    ///     Редирект при логине
    /// </summary>
    public const string LoginRedirectSuccess = "/";

    /// <summary>
    ///     Редирект при ошибке логина
    /// </summary>
    public const string LoginRedirectError = "/login?error=1";

    /// <summary>
    ///     Страница логина
    /// </summary>
    public const string LoginPage = "/login";
}
