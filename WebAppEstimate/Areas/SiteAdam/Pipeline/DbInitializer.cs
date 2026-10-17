using Bogus;
using WebAppEstimate.Areas.SiteAdam.Data.DbSetContext;
using WebAppEstimate.Areas.SiteAdam.Data.Entity.CentrifugalPumps;

namespace WebAppEstimate.Areas.SiteAdam.Pipeline;




/// <summary>
/// Заполняет базу данных SiteAdam начальными демонстрационными данными.
/// Создаёт серии насосов, наборы характеристик и связанные конструкции.
/// </summary>
public class DbInitializer
{
    /// <summary>
    /// Добавляет в базу начальные данные для центробежных насосов:
    /// серии, диаметры рабочего колеса, массу, объём,
    /// трудоёмкость и итоговые конструкции.
    /// </summary>
    /// <param name="context">
    /// Контекст базы данных SiteAdam.
    /// </param>
    public static void SeedData(
        AppDbContextCentrifugalPump context)
    {
        // Стандартные значения размеров.
        // Они повторяются для разных серий,
        // но трудоёмкость для каждой серии отличается.
        var standardImpellers =
            new[] { 100, 150, 200, 250, 300 };

        var standardWeights =
            new[] { 200, 300, 400, 500, 600 };

        var standardVolumes =
            new[] { 40, 60, 80, 100, 120 };

        // Базовые обозначения серий насосов.
        var pumpSeriesNames =
            new[] { "WA", "NT", "MU", "ESM", "SSV" };

        var pumpSeriesList =
            new List<PumpSeries>();

        var faker =
            new Faker();

        // Создаём серии насосов с уникальными названиями.
        foreach (var seriesName in pumpSeriesNames)
        {
            pumpSeriesList.Add(
                new PumpSeries
                {
                    Name =
                        $"{seriesName} " +
                        $"({faker.Random.Replace("Series ##-X")})"
                });
        }

        context.PumpSeries.AddRange(
            pumpSeriesList);

        // Сохраняем серии, чтобы EF Core
        // присвоил им идентификаторы.
        context.SaveChanges();

        // Используется для генерации демонстрационных
        // значений трудоёмкости.
        var random =
            new Random();

        // Для каждой серии формируем собственные диапазоны
        // трудоёмкости.
        foreach (var series in pumpSeriesList)
        {
            double impMin = 1.0;
            double impMax = 11.0;

            double wgtMin = 2.0;
            double wgtMax = 22.0;

            double volMin = 3.0;
            double volMax = 33.0;

            // Каждая серия получает свой диапазон часов.
            switch (series.Name)
            {
                case string s when s.Contains("WA"):
                    impMin = 1.0;
                    impMax = 11.0;
                    wgtMin = 2.0;
                    wgtMax = 22.0;
                    volMin = 3.0;
                    volMax = 33.0;
                    break;

                case string s when s.Contains("NT"):
                    impMin = 2.0;
                    impMax = 12.0;
                    wgtMin = 3.0;
                    wgtMax = 23.0;
                    volMin = 4.0;
                    volMax = 34.0;
                    break;

                case string s when s.Contains("MU"):
                    impMin = 3.0;
                    impMax = 13.0;
                    wgtMin = 4.0;
                    wgtMax = 24.0;
                    volMin = 5.0;
                    volMax = 35.0;
                    break;

                case string s when s.Contains("ESM"):
                    impMin = 4.0;
                    impMax = 14.0;
                    wgtMin = 5.0;
                    wgtMax = 25.0;
                    volMin = 6.0;
                    volMax = 36.0;
                    break;

                case string s when s.Contains("SSV"):
                    impMin = 5.0;
                    impMax = 15.0;
                    wgtMin = 6.0;
                    wgtMax = 26.0;
                    volMin = 7.0;
                    volMax = 37.0;
                    break;

                default:
                    impMin = 5.0;
                    impMax = 15.0;
                    wgtMin = 6.0;
                    wgtMax = 16.0;
                    volMin = 7.0;
                    volMax = 17.0;
                    break;
            }

            // Локальная вспомогательная функция
            // генерирует случайную трудоёмкость
            // в заданном диапазоне.
            double GenerateHour(
                double min,
                double max)
            {
                return Math.Round(
                    random.NextDouble() *
                    (max - min) +
                    min,
                    2);
            }

            // Добавляем наборы рабочих колёс
            // для текущей серии.
            foreach (var size in standardImpellers)
            {
                context.PumpImpellers.Add(
                    new PumpImpeller
                    {
                        PumpSeriesId = series.Id,
                        Impeller = size,
                        Hour = GenerateHour(
                            impMin,
                            impMax)
                    });
            }

            // Добавляем наборы массы
            // для текущей серии.
            foreach (var weight in standardWeights)
            {
                context.PumpWeights.Add(
                    new PumpWeight
                    {
                        PumpSeriesId = series.Id,
                        Weight = weight,
                        Hour = GenerateHour(
                            wgtMin,
                            wgtMax)
                    });
            }

            // Добавляем наборы объёма
            // для текущей серии.
            foreach (var volume in standardVolumes)
            {
                context.PumpVolumes.Add(
                    new PumpVolume
                    {
                        PumpSeriesId = series.Id,
                        Volume = volume,
                        Hour = GenerateHour(
                            volMin,
                            volMax)
                    });
            }
        }

        // Сохраняем созданные характеристики,
        // чтобы получить их идентификаторы.
        context.SaveChanges();

        // Для каждой серии создаём одну
        // связанную демонстрационную конструкцию.
        foreach (var series in pumpSeriesList)
        {
            var firstImpeller =
                context.PumpImpellers
                    .First(i =>
                        i.PumpSeriesId == series.Id);

            var firstWeight =
                context.PumpWeights
                    .First(w =>
                        w.PumpSeriesId == series.Id);

            var firstVolume =
                context.PumpVolumes
                    .First(v =>
                        v.PumpSeriesId == series.Id);

            context.PumpCentrifugalDesigns.Add(
                new PumpCentrifugalDesign
                {
                    PumpSeriesId = series.Id,
                    PumpImpellerId = firstImpeller.Id,
                    PumpWeightId = firstWeight.Id,
                    PumpVolumeId = firstVolume.Id
                });
        }

        // Финально сохраняем созданные конструкции.
        context.SaveChanges();
    }
}











