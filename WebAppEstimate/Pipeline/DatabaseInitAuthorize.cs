using Microsoft.AspNetCore.Identity;
using WebAppEstimate.Data.DbContext;
using WebAppEstimate.Data.Entity;

namespace WebAppEstimate.Pipeline;

public static class DatabaseInitAuthorize
{
    public static IApplicationBuilder InitializeAuthorize(this IApplicationBuilder app)
    {
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

            /*if (context.UserAuthorizations.Any())
                return app;*/

            var hasher =
                new PasswordHasher<UserAuthz>();

            //-------------
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
            //------------


            //-------------
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
            //--------------


            //------------
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

            //-------------


            //----------------
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
            //----------------


            context.SaveChanges();
        }

        return app;
    }
}