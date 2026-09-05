using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Data.DbContext;
using WebAppEstimate.Models.User;
using WebAppEstimate.Services;

namespace WebAppEstimate.Controllers;

public class AccountController : Controller
{
    //---Database
    private readonly AppDbContext _context;
    private readonly IValidator<UserLogin> _validator;
    private  readonly IJwtTokenService _jwtTokenService;

    // Внедряем ваш Fluent-валидатор через DI-контейнер
    //
    public AccountController(IValidator<UserLogin> validator, AppDbContext context,IJwtTokenService jwtTokenService)
    {
        _validator = validator;
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    //--- GET: 
    [HttpGet]
    public IActionResult Account()
    {
        // Если пользователь уже вошел, отправляем его на главную
        if (User.Identity != null && User.Identity.IsAuthenticated)
            return RedirectToAction("Start", "Start", new { area = "" });
        return View();
    }

    //--- POST: 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Account(UserLogin userLogin)
    {
        // 1. Запускаем вашу валидацию из UserLoginValid
        var validationResult = await _validator.ValidateAsync(userLogin);

        //---
        if (!validationResult.IsValid)
        {
            // Переносим ошибки из FluentValidation в ModelState, чтобы отобразить в HTML
            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            return View(userLogin);
        }

        //---
        // 2.  заменим на запрос к AppDbContext
        var user = await _context.UserAuthorizations
            .SingleOrDefaultAsync(u => u.Login == userLogin.Login && u.Password == userLogin.Password);

        //--проверка
        if (user != null)
        {
            var token = _jwtTokenService.CreateToken(user);

            Response.Cookies.Append(
                "access_token",
                token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(2)
                });

            return RedirectToAction(
                "Start",
                "Start",
                new { area = "" });
            
        }

        // Если логин/пароль не подошли к базе данных
        ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
        return View(userLogin);
    }


    //---GET: /Account/AccountLoginOut
    public async Task<ActionResult> AccountLoginOut()
    {
        Response.Cookies.Delete("access_token");

        return RedirectToAction(
            "Index",
            "Home",
            new { area = "" });
    }
}




/*public class AccountController : Controller
{
    //---Database
    private readonly AppDbContext _context;
    private readonly IValidator<UserLogin> _validator;

    // Внедряем ваш Fluent-валидатор через DI-контейнер
    //
    public AccountController(IValidator<UserLogin> validator, AppDbContext context)
    {
        _validator = validator;
        _context = context;
    }

    //--- GET: 
    [HttpGet]
    public IActionResult Account()
    {
        // Если пользователь уже вошел, отправляем его на главную
        if (User.Identity != null && User.Identity.IsAuthenticated)
            return RedirectToAction("Start", "Start", new { area = "" });
        return View();
    }

    //--- POST: 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Account(UserLogin userLogin)
    {
        // 1. Запускаем вашу валидацию из UserLoginValid
        var validationResult = await _validator.ValidateAsync(userLogin);

        //---
        if (!validationResult.IsValid)
        {
            // Переносим ошибки из FluentValidation в ModelState, чтобы отобразить в HTML
            foreach (var error in validationResult.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            return View(userLogin);
        }

        //---
        // 2.  заменим на запрос к AppDbContext
        var user = await _context.UserAuthorizations
            .SingleOrDefaultAsync(u => u.Login == userLogin.Login && u.Password == userLogin.Password);

        //--проверка
        if (user != null)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Login),
                new(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Start", "Start", new { area = "" });
        }

        // Если логин/пароль не подошли к базе данных
        ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
        return View(userLogin);
    }


    //---GET: /Account/AccountLoginOut
    public async Task<ActionResult> AccountLoginOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //---
        return RedirectToAction("Index", "Home", new { area = "" });
    }
}*/