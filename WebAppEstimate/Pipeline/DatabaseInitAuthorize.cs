using Microsoft.AspNetCore.Identity;
using WebAppEstimate.Data.DbContext;
using WebAppEstimate.Data.Entity;

namespace WebAppEstimate.Pipeline;

/// <summary>
/// Выполняет первоначальную инициализацию базы данных авторизации.
/// Создаёт базу при её отсутствии и добавляет стандартных пользователей,
/// если соответствующих учётных записей ещё нет.
/// </summary>

public static class DatabaseInitAuthorize
{
    /// <summary>
    /// Инициализирует базу данных авторизации приложения.
    /// При необходимости пересоздаёт базу, создаёт её структуру
    /// и добавляет стандартные учётные записи с хешированными паролями.
    /// </summary>
    /// <param name="app">
    /// Конвейер ASP.NET Core, через который получается доступ
    /// к зарегистрированным сервисам приложения.
    /// </param>
    /// <returns>
    /// Исходный экземпляр <see cref="IApplicationBuilder"/>.
    /// </returns>
    /// 
    public static IApplicationBuilder InitializeAuthorize(this IApplicationBuilder app)
    {
        // Создаём отдельную область DI,
        // чтобы получить DbContext и конфигурацию приложения.
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var context =
                scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var configuration =
                scope.ServiceProvider
                    .GetRequiredService<IConfiguration>();

            // Флаг используется только для принудительного
            // удаления и повторного создания базы авторизации.
            var resetDatabase =
                configuration.GetValue<bool>(
                    "ResetAuthorizeDatabase");

            if (resetDatabase) context.Database.EnsureDeleted();

            // Создаём базу и таблицы, если они ещё не существуют.
            context.Database.EnsureCreated();

            // PasswordHasher используется для безопасного хранения
            // паролей вместо сохранения их в открытом виде.
            var hasher =
                new PasswordHasher<UserAuthz>();

            // Создаём администратора только в том случае,
            // если такой пользователь ещё отсутствует.
            if (!context.UserAuthorizations.Any(x => x.Login == "admins"))
            {
                var admin = new UserAuthz
                {
                    Id = 1,
                    Login = "admins",
                    Role = "Admin"
                };

                admin.Password =
                    hasher.HashPassword(admin, "admins");

                context.UserAuthorizations.Add(admin);
            }
            
            // Пользователь функциональной области SiteAdam.
            if (!context.UserAuthorizations.Any(x => x.Login == "userAdam"))
            {
                var adam = new UserAuthz
                {
                    Id = 2,
                    Login = "userAdam",
                    Role = "Adam"
                };

                adam.Password =
                    hasher.HashPassword(adam, "adam123");

                context.UserAuthorizations.Add(adam);
            }
            


            // Пользователь функциональной области SiteBram.
            if (!context.UserAuthorizations.Any(x => x.Login == "userBram"))
            {
                var bram = new UserAuthz
                {
                    Id = 3,
                    Login = "userBram",
                    Role = "Bram"
                };

                bram.Password =
                    hasher.HashPassword(bram, "bram123");

                context.UserAuthorizations.Add(bram);
            }

            
            // Пользователь функциональной области SiteCerber.
            if (!context.UserAuthorizations.Any(x => x.Login == "userCerber"))
            {
                var cerber = new UserAuthz
                {
                    Id = 4,
                    Login = "userCerber",
                    Role = "Cerber"
                };

                cerber.Password =
                    hasher.HashPassword(
                        cerber,
                        "cerber123");

                context.UserAuthorizations.Add(cerber);
            }
            // Сохраняем только новые учётные записи,
            // которые были добавлены при текущем запуске.
            
            context.SaveChanges();
        }

        return app;
    }
}