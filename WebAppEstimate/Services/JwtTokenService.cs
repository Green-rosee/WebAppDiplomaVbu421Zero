using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using WebAppEstimate.Data.Entity;

namespace WebAppEstimate.Services;

/// <summary>
/// Сервис генерации JWT-токенов.
/// Формирует токен с логином и ролью пользователя
/// и подписывает его симметричным ключом.
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    /// <summary>
    /// Создаёт JWT-токен для авторизованного пользователя.
    /// </summary>
    /// <param name="user">
    /// Пользователь, данные которого будут записаны в claims токена.
    /// </param>
    /// <returns>
    /// Готовый JWT-токен в строковом формате.
    /// </returns>
    public string CreateToken(UserAuthz user)
    {
        // Claims содержат основные данные пользователя,
        // которые затем используются системой авторизации.
        var claims = new List<Claim>
        {
            new(
                ClaimTypes.Name,
                user.Login),

            new(
                ClaimTypes.Role,
                user.Role)
        };

        // Создаём данные для цифровой подписи JWT.
        // Используется симметричный ключ и алгоритм HMAC SHA-256.
        var credentials =
            new SigningCredentials(
                AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256);

        // Формируем JWT с issuer, audience,
        // пользовательскими claims и временем жизни токена.
        var token =
            new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires:
                DateTime.UtcNow.AddMinutes(2),
                signingCredentials: credentials);

        // Преобразуем объект JwtSecurityToken
        // в строку, которая затем записывается в cookie.
        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}







                                                  