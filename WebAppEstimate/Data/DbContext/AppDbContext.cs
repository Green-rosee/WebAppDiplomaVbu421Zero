using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Data.Entity;

namespace WebAppEstimate.Data.DbContext;

public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UserAuthz> UserAuthorizations { get; set; }

    //
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserAuthz>().HasData(
            new UserAuthz
            {
                Id = 1,
                Login = "admins",
                Password = "admins",
                Role = "Admin"
            },
            new UserAuthz
            {
                Id = 2,
                Login = "userAdam",
                Password = "adam123",
                Role = "Adam"
            },
            new UserAuthz
            {
                Id = 3,
                Login = "userBram",
                Password = "bram123",
                Role = "Bram"
            }
        );
    }
}