using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteBram.Data.DbSetContext;
using WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;

namespace WebAppEstimate.Tests;



/// <summary>
/// Интеграционные тесты SiteBram для проверки
/// взаимодействия EF Core с PostgreSQL.
/// </summary>
public class WinchAnchorPostgresIntegrationTests
{
    /// <summary>
    /// Проверяет, что сущность серии якорной лебёдки
    /// корректно сохраняется в PostgreSQL
    /// и затем считывается обратно из базы данных.
    /// </summary>
    [Fact]
    public async Task PostgreSql_ShouldSaveAndReadWinchSeries()
    {
        // Arrange
        // Получаем строку подключения к тестовой PostgreSQL-базе
        // из переменной окружения.
        // Если переменная отсутствует, используется локальная БД.
        var connectionString =
            Environment.GetEnvironmentVariable(
                "TEST_POSTGRES_CONNECTION")
            ??
            "Host=localhost;" +
            "Port=5432;" +
            "Database=webapp_bram_test;" +
            "Username=webapp;" +
            "Password=webapp_password";

        // Настраиваем DbContext для работы через Npgsql/PostgreSQL.
        var options =
            new DbContextOptionsBuilder<AppDbContextWinchAnchor>()
                .UseNpgsql(connectionString)
                .Options;

        await using var context =
            new AppDbContextWinchAnchor(options);

        // Создаём структуру базы данных,
        // если она ещё не существует.
        await context.Database
            .EnsureCreatedAsync();

        var seriesId =
            Guid.NewGuid();

        var series =
            new WinchAnchorSeries
            {
                Id = seriesId,
                Name = "IntegrationTestSeries"
            };

        // Act
        // Сохраняем тестовую запись в реальную PostgreSQL-базу.
        context.WinchAnchorSeries.Add(series);

        await context.SaveChangesAsync();

        // Читаем сохранённую запись обратно из базы.
        var result =
            await context.WinchAnchorSeries
                .FirstOrDefaultAsync(
                    x => x.Id == seriesId);

        // Assert
        // Проверяем, что запись действительно была сохранена
        // и данные не изменились при чтении из PostgreSQL.
        Assert.NotNull(result);

        Assert.Equal(
            "IntegrationTestSeries",
            result.Name);

        // Cleanup
        // Удаляем тестовые данные,
        // чтобы тест не загрязнял базу после выполнения.
        context.WinchAnchorSeries.Remove(result);

        await context.SaveChangesAsync();
    }
}











