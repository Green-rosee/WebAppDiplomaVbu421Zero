using WebAppEstimate.Data.Entity;

namespace WebAppEstimate.Services;


/// <summary>
/// Определяет сервис для создания JWT-токенов
/// авторизованных пользователей.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Создаёт JWT-токен для указанного пользователя.
    /// В токен добавляются логин пользователя и его роль.
    /// </summary>
    /// <param name="user">
    /// Пользователь, для которого формируется токен.
    /// </param>
    /// <returns>
    /// Строковое представление JWT-токена.
    /// </returns>
    string CreateToken(UserAuthz user);
}









