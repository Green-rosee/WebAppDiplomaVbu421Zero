using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Data.DbContext;
using WebAppEstimate.Data.Entity;
using WebAppEstimate.Models.User;
using WebAppEstimate.Services;

namespace WebAppEstimate.Controllers;

/// <summary>
///     Контроллер отвечает за вход и выход пользователя из системы.
///     Выполняет проверку введённых данных, проверку пароля,
///     создание JWT-токена и работу с cookie авторизации.
/// </summary>
public class AccountController : Controller
{
    private readonly AppDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IValidator<UserLogin> _validator;

    /// <summary>
    ///     Создаёт контроллер авторизации и получает необходимые
    ///     зависимости через встроенный DI-контейнер ASP.NET Core.
    /// </summary>
    /// <param name="validator">
    ///     FluentValidation-валидатор данных формы входа.
    /// </param>
    /// <param name="context">
    ///     Контекст базы данных пользователей.
    /// </param>
    /// <param name="jwtTokenService">
    ///     Сервис создания JWT-токенов.
    /// </param>
    public AccountController(
        IValidator<UserLogin> validator,
        AppDbContext context,
        IJwtTokenService jwtTokenService)
    {
        _validator = validator;
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    ///     Отображает страницу авторизации.
    ///     Если пользователь уже авторизован,
    ///     перенаправляет его на стартовую страницу приложения.
    /// </summary>
    /// <returns>
    ///     Представление формы входа либо перенаправление
    ///     на стартовую страницу.
    /// </returns>
    [HttpGet]
    public IActionResult Account()
    {
        if (User.Identity != null &&
            User.Identity.IsAuthenticated)
            return RedirectToAction(
                "Start",
                "Start",
                new { area = "" });

        return View();
    }

    /// <summary>
    ///     Обрабатывает отправленную пользователем форму авторизации.
    ///     Проверяет корректность данных, ищет пользователя в базе,
    ///     проверяет хеш пароля и при успешной авторизации
    ///     создаёт JWT-токен.
    /// </summary>
    /// <param name="userLogin">
    ///     Логин и пароль, введённые пользователем.
    /// </param>
    /// <returns>
    ///     При успешном входе — перенаправление на стартовую страницу.
    ///     При ошибке — форма входа с сообщением об ошибке.
    /// </returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Account(
        UserLogin userLogin)
    {
        // Проверяем введённые данные через FluentValidation.
        var validationResult =
            await _validator.ValidateAsync(userLogin);

        if (!validationResult.IsValid)
        {
            // Переносим ошибки FluentValidation в ModelState,
            // чтобы Razor View мог отобразить их пользователю.
            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(
                    error.PropertyName,
                    error.ErrorMessage);

            return View(userLogin);
        }

        // Ищем пользователя в базе данных по введённому логину.
        var user =
            await _context.UserAuthorizations
                .SingleOrDefaultAsync(u => u.Login == userLogin.Login);

        if (user != null)
        {
            var hasher =
                new PasswordHasher<UserAuthz>();

            // Сравниваем введённый пароль
            // с хешем, сохранённым в базе данных.
            var passwordResult =
                hasher.VerifyHashedPassword(
                    user,
                    user.Password,
                    userLogin.Password);

            if (passwordResult ==
                PasswordVerificationResult.Success ||
                passwordResult ==
                PasswordVerificationResult.SuccessRehashNeeded)
            {
                // Формируем JWT с данными пользователя и его ролью.
                var token =
                    _jwtTokenService.CreateToken(user);

                // Сохраняем JWT в HttpOnly cookie.
                // JavaScript в браузере не получает прямого доступа
                // к этому cookie.
                Response.Cookies.Append(
                    "access_token",
                    token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires =
                            DateTimeOffset.UtcNow
                                .AddMinutes(20)
                    });

                return RedirectToAction(
                    "Start",
                    "Start",
                    new { area = "" });
            }
        }

        // Используем общее сообщение, чтобы не раскрывать,
        // существует ли конкретный логин в системе.
        ModelState.AddModelError(
            string.Empty,
            "Неверный логин или пароль.");

        return View(userLogin);
    }

    /// <summary>
    ///     Выполняет выход пользователя из системы.
    ///     Удаляет JWT-cookie и перенаправляет пользователя
    ///     на главную страницу приложения.
    /// </summary>
    /// <returns>
    ///     Перенаправление на главную страницу.
    /// </returns>
    public ActionResult AccountLoginOut()
    {
        Response.Cookies.Delete(
            "access_token");

        return RedirectToAction(
            "Index",
            "Home",
            new { area = "" });
    }
}

