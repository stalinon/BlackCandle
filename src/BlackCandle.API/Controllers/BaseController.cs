using BlackCandle.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace BlackCandle.API.Controllers;

/// <summary>
///     Базовый контроллер для оборачивания
/// </summary>
[ApiController]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    ///     Успех
    /// </summary>
    protected ActionResult<OperationResult<T>> Success<T>(T data)
        => Ok(OperationResult<T>.Success(data));

    /// <summary>
    ///     Ошибка
    /// </summary>
    protected ActionResult<OperationResult<T>> Fail<T>(string error)
        => BadRequest(OperationResult<T>.Failure(error));
}
