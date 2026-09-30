using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteBram.Data.DbSetContext;
using WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;

namespace WebAppEstimate.Tests;

public class WinchAnchorPostgresIntegrationTests
{
    [Fact]
    public async Task PostgreSql_ShouldSaveAndReadWinchSeries()
    {
        //------------ Arrange

        var connectionString =
            Environment.GetEnvironmentVariable("TEST_POSTGRES_CONNECTION")
            ??
            "Host=localhost;" +
            "Port=5432;" +
            "Database=webapp_bram_test;" +
            "Username=webapp;" +
            "Password=webapp_password";

        var options =
            new DbContextOptionsBuilder<AppDbContextWinchAnchor>()
                .UseNpgsql(connectionString)
                .Options;
        //--------------------
        await using var context =
            new AppDbContextWinchAnchor(options);

        await context.Database.EnsureCreatedAsync();

        var seriesId = Guid.NewGuid();

        var series = new WinchAnchorSeries
        {
            Id = seriesId,
            Name = "IntegrationTestSeries"
        };

        //------------------ Act
        context.WinchAnchorSeries.Add(series);

        await context.SaveChangesAsync();

        var result =
            await context.WinchAnchorSeries
                .FirstOrDefaultAsync(x => x.Id == seriesId);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(
            "IntegrationTestSeries",
            result.Name);

        // Cleanup
        context.WinchAnchorSeries.Remove(result);

        await context.SaveChangesAsync();
    }
}