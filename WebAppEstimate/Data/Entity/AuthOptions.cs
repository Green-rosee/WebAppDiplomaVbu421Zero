using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebAppEstimate.Data.Entity;


/// <summary>
/// Содержит основные параметры, используемые
/// для создания и проверки JWT-токенов.
/// </summary>
public class AuthOptions
{
    /// <summary>
    /// Издатель JWT-токена.
    /// Используется для проверки, каким приложением был создан токен.
    /// </summary>
    public const string ISSUER = "MyAuthServer";

    /// <summary>
    /// Получатель JWT-токена.
    /// Определяет приложение, для которого предназначен токен.
    /// </summary>
    public const string AUDIENCE = "MyAuthClient";

    /// <summary>
    /// Секретный ключ, используемый для цифровой подписи JWT.
    /// </summary>
    private const string KEY =
        "mysupersecret_secretsecretsecretkey!123";

    /// <summary>
    /// Создаёт симметричный ключ безопасности
    /// на основе секретной строки KEY.
    /// </summary>
    /// <returns>
    /// Объект <see cref="SymmetricSecurityKey"/>,
    /// используемый при подписи и проверке JWT-токена.
    /// </returns>
    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(KEY));
    }
}













