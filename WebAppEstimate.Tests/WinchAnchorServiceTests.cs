using Microsoft.EntityFrameworkCore;
using WebAppEstimate.Areas.SiteBram.Data.DbSetContext;
using WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;
using WebAppEstimate.Areas.SiteBram.Services;

namespace WebAppEstimate.Tests;



/// <summary>
/// Модульные тесты сервиса подбора якорной лебёдки.
/// Проверяют алгоритм выбора наиболее подходящей конструкции
/// по заданным пользователем параметрам.
/// </summary>
public class WinchAnchorServiceTests
{
    /// <summary>
    /// Проверяет, что сервис выбирает конструкцию
    /// с минимальным относительным отклонением
    /// от заданных параметров.
    /// </summary>
    [Fact]
    public async Task FindDesignAsync_ShouldReturnNearestDesign()
    {
        // Arrange
        // Создаём отдельную InMemory-базу для текущего теста.
        var options =
            new DbContextOptionsBuilder<AppDbContextWinchAnchor>()
                .UseInMemoryDatabase(
                    databaseName: Guid.NewGuid().ToString())
                .Options;

        await using var context =
            new AppDbContextWinchAnchor(options);

        var seriesId = Guid.NewGuid();

        var series = new WinchAnchorSeries
        {
            Id = seriesId,
            Name = "TestSeries"
        };

        // Создаём несколько вариантов конструкции
        // с различными параметрами.
        var design1 = new WinchAnchorDesign
        {
            Id = Guid.NewGuid(),
            Name = "Design-1",
            WinchAnchorSeriesId = seriesId,
            ValueKg = 1000,
            ValueMm = 100,
            ValueKgMm = 22,
            Hour = 300
        };

        var design2 = new WinchAnchorDesign
        {
            Id = Guid.NewGuid(),
            Name = "Design-2",
            WinchAnchorSeriesId = seriesId,
            ValueKg = 1500,
            ValueMm = 150,
            ValueKgMm = 27,
            Hour = 500
        };

        var design3 = new WinchAnchorDesign
        {
            Id = Guid.NewGuid(),
            Name = "Design-3",
            WinchAnchorSeriesId = seriesId,
            ValueKg = 2500,
            ValueMm = 250,
            ValueKgMm = 37,
            Hour = 800
        };

        context.WinchAnchorSeries.Add(series);

        context.WinchAnchorDesigns.AddRange(
            design1,
            design2,
            design3);

        await context.SaveChangesAsync();

        var service =
            new WinchAnchorService(context);

        // Act
        // Выполняем подбор для промежуточных параметров.
        var result =
            await service.FindDesignAsync(
                seriesId,
                1300,
                145,
                25);

        // Assert
        // Проверяем, что алгоритм выбрал
        // ближайший вариант Design-2.
        Assert.NotNull(result);

        Assert.Equal(
            "Design-2",
            result.Name);

        Assert.Equal(
            1500,
            result.ValueKg);

        Assert.Equal(
            150,
            result.ValueMm);

        Assert.Equal(
            27,
            result.ValueKgMm);
    }

    /// <summary>
    /// Проверяет, что сервис возвращает null,
    /// если для выбранной серии отсутствуют конструкции.
    /// </summary>
    [Fact]
    public async Task FindDesignAsync_ShouldReturnNull_WhenSeriesHasNoDesigns()
    {
        // Arrange
        var options =
            new DbContextOptionsBuilder<AppDbContextWinchAnchor>()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString())
                .Options;

        await using var context =
            new AppDbContextWinchAnchor(options);

        var seriesId = Guid.NewGuid();

        var series = new WinchAnchorSeries
        {
            Id = seriesId,
            Name = "EmptySeries"
        };

        // Добавляем серию, но не добавляем для неё конструкции.
        context.WinchAnchorSeries.Add(series);

        await context.SaveChangesAsync();

        var service =
            new WinchAnchorService(context);

        // Act
        var result =
            await service.FindDesignAsync(
                seriesId,
                1300,
                145,
                25);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Проверяет, что при подборе учитываются только конструкции
    /// выбранной серии, даже если в другой серии существует
    /// более точное совпадение параметров.
    /// </summary>
    [Fact]
    public async Task FindDesignAsync_ShouldIgnoreOtherSeries()
    {
        // Arrange
        var options =
            new DbContextOptionsBuilder<AppDbContextWinchAnchor>()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString())
                .Options;

        await using var context =
            new AppDbContextWinchAnchor(options);

        var targetSeriesId = Guid.NewGuid();
        var otherSeriesId = Guid.NewGuid();

        var targetSeries = new WinchAnchorSeries
        {
            Id = targetSeriesId,
            Name = "TargetSeries"
        };

        var otherSeries = new WinchAnchorSeries
        {
            Id = otherSeriesId,
            Name = "OtherSeries"
        };

        var targetDesign = new WinchAnchorDesign
        {
            Id = Guid.NewGuid(),
            Name = "TargetDesign",
            WinchAnchorSeriesId = targetSeriesId,

            ValueKg = 1500,
            ValueMm = 150,
            ValueKgMm = 27,

            Hour = 500
        };

        var otherDesign = new WinchAnchorDesign
        {
            Id = Guid.NewGuid(),
            Name = "OtherDesign",
            WinchAnchorSeriesId = otherSeriesId,

            // Конструкция другой серии специально имеет
            // практически идеальное совпадение параметров.
            ValueKg = 1300,
            ValueMm = 145,
            ValueKgMm = 25,

            Hour = 400
        };

        context.WinchAnchorSeries.AddRange(
            targetSeries,
            otherSeries);

        context.WinchAnchorDesigns.AddRange(
            targetDesign,
            otherDesign);

        await context.SaveChangesAsync();

        var service =
            new WinchAnchorService(context);

        // Act
        var result =
            await service.FindDesignAsync(
                targetSeriesId,
                1300,
                145,
                25);

        // Assert
        // Конструкция другой серии не должна участвовать в подборе.
        Assert.NotNull(result);

        Assert.Equal(
            "TargetDesign",
            result.Name);

        Assert.Equal(
            targetSeriesId,
            result.WinchAnchorSeriesId);
    }

    /// <summary>
    /// Проверяет, что при наличии полного совпадения параметров
    /// сервис выбирает именно эту конструкцию.
    /// </summary>
    [Fact]
    public async Task FindDesignAsync_ShouldReturnExactMatch_WhenExactDesignExists()
    {
        // Arrange
        var options =
            new DbContextOptionsBuilder<AppDbContextWinchAnchor>()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString())
                .Options;

        await using var context =
            new AppDbContextWinchAnchor(options);

        var seriesId = Guid.NewGuid();

        var series = new WinchAnchorSeries
        {
            Id = seriesId,
            Name = "ExactSeries"
        };

        var design1 = new WinchAnchorDesign
        {
            Id = Guid.NewGuid(),
            Name = "NearDesign",
            WinchAnchorSeriesId = seriesId,

            ValueKg = 1500,
            ValueMm = 150,
            ValueKgMm = 27,

            Hour = 500
        };

        var design2 = new WinchAnchorDesign
        {
            Id = Guid.NewGuid(),
            Name = "ExactDesign",
            WinchAnchorSeriesId = seriesId,

            ValueKg = 1300,
            ValueMm = 145,
            ValueKgMm = 25,

            Hour = 450
        };

        context.WinchAnchorSeries.Add(series);

        context.WinchAnchorDesigns.AddRange(
            design1,
            design2);

        await context.SaveChangesAsync();

        var service =
            new WinchAnchorService(context);

        // Act
        var result =
            await service.FindDesignAsync(
                seriesId,
                1300,
                145,
                25);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(
            "ExactDesign",
            result.Name);

        Assert.Equal(
            1300,
            result.ValueKg);

        Assert.Equal(
            145,
            result.ValueMm);

        Assert.Equal(
            25,
            result.ValueKgMm);
    }
}











