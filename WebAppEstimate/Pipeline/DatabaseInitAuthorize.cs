using Microsoft.AspNetCore.Identity;
using WebAppEstimate.Data.DbContext;
using WebAppEstimate.Data.Entity;

namespace WebAppEstimate.Pipeline;

public static class DatabaseInitAuthorize
{
    public static IApplicationBuilder InitializeAuthorize(this IApplicationBuilder app)
    {
        /*using (var scope = app.ApplicationServices.CreateScope())
        {
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                var configuration = services.GetRequiredService<IConfiguration>();

                var resetDatabase = configuration.GetValue<bool>("ResetAuthorizeDatabase");

                if (resetDatabase) context.Database.EnsureDeleted();

                //---я закоментил так как проект в докере
                //context.Database.EnsureDeleted();
                //
                //---
                context.Database.EnsureCreated();
            }
            catch (Exception e)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(e, "Ошибка при инициализации базы данных.");
            }
        }

        return app;*/

        //------------
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var context =
                scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var configuration =
                scope.ServiceProvider
                    .GetRequiredService<IConfiguration>();

            var resetDatabase =
                configuration.GetValue<bool>(
                    "ResetAuthorizeDatabase");

            if (resetDatabase) context.Database.EnsureDeleted();

            context.Database.EnsureCreated();

            if (context.UserAuthorizations.Any())
                return app;

            var hasher =
                new PasswordHasher<UserAuthz>();

            var admin = new UserAuthz
            {
                Id = 1,
                Login = "admins",
                Role = "Admin"
            };

            admin.Password =
                hasher.HashPassword(admin, "admins");


            var adam = new UserAuthz
            {
                Id = 2,
                Login = "userAdam",
                Role = "Adam"
            };

            adam.Password =
                hasher.HashPassword(adam, "adam123");


            var bram = new UserAuthz
            {
                Id = 3,
                Login = "userBram",
                Role = "Bram"
            };

            bram.Password =
                hasher.HashPassword(bram, "bram123");


            context.UserAuthorizations.AddRange(
                admin,
                adam,
                bram);

            context.SaveChanges();
        }

        return app;
    }
}