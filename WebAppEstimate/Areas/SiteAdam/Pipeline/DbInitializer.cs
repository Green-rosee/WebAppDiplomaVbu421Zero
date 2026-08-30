using Bogus;
using WebAppEstimate.Areas.SiteAdam.Data.DbSetContext;
using WebAppEstimate.Areas.SiteAdam.Data.Entity.CentrifugalPumps;

namespace WebAppEstimate.Areas.SiteAdam.Pipeline;

public class DbInitializer
{
    public static void SeedData(AppDbContextCentrifugalPump context)
    {
        // Очищаем и пересоздаем базу данных (как у вас в коде)

        //---закоментил так как поместил в докер
        // context.Database.EnsureDeleted();
        // context.Database.EnsureCreated();

        // Проверяем, есть ли уже данные (на всякий случай)
        //if (context.PumpSeries.Any()) return;

        // Фиксированные наборы РАЗМЕРОВ (чтобы они гарантированно повторялись в разных сериях)
        var standardImpellers = new[] { 100, 150, 200, 250, 300 };
        var standardWeights = new[] { 200, 300, 400, 500, 600 };
        var standardVolumes = new[] { 40, 60, 80, 100, 120 };

        //--- Инициализируем Faker для серий насосов
        var asArrPumpSeriesName = new[] { "WA", "NT", "MU", "ESM", "SSV" };
        var pumpSeriesList = new List<PumpSeries>();
        var faker = new Faker();
        foreach (var series in asArrPumpSeriesName)
            pumpSeriesList.Add(new PumpSeries
            {
                Name = $"{series} ({faker.Random.Replace("Series ##-X")})"
            });

        context.PumpSeries.AddRange(pumpSeriesList);
        context.SaveChanges(); // Сохраняем, чтобы получить Id для серий

        // Генератор случайных чисел для часов (Hour)
        var random = new Random();

        // Проходим по каждой созданной серии и наполняем её характеристиками
        foreach (var series in pumpSeriesList)
        {
            // Переменные для хранения индивидуальных границ текущей серии
            // Шаг 1: Создаем переменные для диапазонов (для каждого типа свои)
            double impMin = 1.0, impMax = 11.0;
            double wgtMin = 2.0, wgtMax = 22.0;
            double volMin = 3.0, volMax = 33.0;

            // Для каждой серии берем стандартные размеры, но генерируем УНИКАЛЬНЫЕ часы
            switch (series.Name)
            {
                case string s when s.Contains("WA"):
                    //---
                    impMin = 1.0;
                    impMax = 11.0;
                    wgtMin = 2.0;
                    wgtMax = 22.0;
                    volMin = 3.0;
                    volMax = 33.0;
                    break;
                case string s when s.Contains("NT"):
                    //---
                    impMin = 2.0;
                    impMax = 12.0;
                    wgtMin = 3.0;
                    wgtMax = 23.0;
                    volMin = 4.0;
                    volMax = 34.0;
                    break;
                case string s when s.Contains("MU"):
                    //---
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
                    // Дефолтные значения, если серия не подошла под условия выше
                    impMin = 5.0;
                    impMax = 15.0;
                    wgtMin = 6.0;
                    wgtMax = 16.0;
                    volMin = 7.0;
                    volMax = 17.0;
                    break;
            }

            // Вспомогательная функция для генерации часов по вычисленным границам
            double GenerateHour(double min, double max)
            {
                return Math.Round(random.NextDouble() * (max - min) + min, 2);
            }

            // 1. Наполняем колеса (Impeller)
            foreach (var size in standardImpellers)
                context.PumpImpellers.Add(new PumpImpeller
                {
                    PumpSeriesId = series.Id,
                    Impeller = size,
                    // Случайные часы от 1.0 до 5.0 с округлением до 1 знака
                    Hour = GenerateHour(impMin, impMax)
                });

            // 2. Наполняем вес (Weight)
            foreach (var weight in standardWeights)
                context.PumpWeights.Add(new PumpWeight
                {
                    PumpSeriesId = series.Id,
                    Weight = weight,
                    Hour = GenerateHour(wgtMin, wgtMax)
                });

            // 3. Наполняем объем (Volume)
            foreach (var volume in standardVolumes)
                context.PumpVolumes.Add(new PumpVolume
                {
                    PumpSeriesId = series.Id,
                    Volume = volume,
                    Hour = GenerateHour(volMin, volMax)
                });
        }

        // Сохраняем характеристики, чтобы они получили свои Id
        context.SaveChanges();

        // 4. Теперь создаем финальные сборки (PumpCentrifugalDesign)
        // Для каждой серии свяжем первое колесо, первый вес и первый объем в готовый дизайн
        foreach (var series in pumpSeriesList)
        {
            // Вытаскиваем только что созданные характеристики именно для этой серии
            var firstImpeller = context.PumpImpellers.First(i => i.PumpSeriesId == series.Id);
            var firstWeight = context.PumpWeights.First(w => w.PumpSeriesId == series.Id);
            var firstVolume = context.PumpVolumes.First(v => v.PumpSeriesId == series.Id);

            context.PumpCentrifugalDesigns.Add(new PumpCentrifugalDesign
            {
                PumpSeriesId = series.Id,
                PumpImpellerId = firstImpeller.Id,
                PumpWeightId = firstWeight.Id,
                PumpVolumeId = firstVolume.Id
            });
        }

        // Финальное сохранение сборок
        context.SaveChanges();
    }
}

/*public static void SeedData(AppDbContextCentrifugalPump context)
    {
        // Очищаем и пересоздаем базу данных (как у вас в коде)
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Проверяем, есть ли уже данные (на всякий случай)
        if (context.PumpSeries.Any()) return;

        // Фиксированные наборы РАЗМЕРОВ (чтобы они гарантированно повторялись в разных сериях)
        var standardImpellers = new[] { 100, 150, 200, 250, 300 };
        var standardWeights = new[] { 200, 300, 400, 500, 600 };
        var standardVolumes = new[] { 40, 60, 80, 100, 120 };

        //--- Инициализируем Faker для серий насосов
        /*var seriesFaker = new Faker<PumpSeries>("ru") // "ru" для русскоязычных названий
            .RuleFor(s => s.Name,
                f => $"Насос Центробежный {f.Commerce.ProductName()} (Серия {f.Random.Replace("##-X")})");#1#
        // Генерируем 5 тестовых серий
        /*var pumpSeriesList = seriesFaker.Generate(5);#1#

        var asArrPumpSeriesName = new[] { "WA", "NT", "MU", "ESM", "SSV" };
        var pumpSeriesList = new List<PumpSeries>();
        var faker = new Faker("en");
        foreach (var series in asArrPumpSeriesName)
        {
            pumpSeriesList.Add(new PumpSeries
            {
                Name = $"{series} ({faker.Random.Replace("Series ##-X")})"
            });
        }

        context.PumpSeries.AddRange(pumpSeriesList);
        context.SaveChanges(); // Сохраняем, чтобы получить Id для серий

        // Генератор случайных чисел для часов (Hour)
        var random = new Random();

        // Проходим по каждой созданной серии и наполняем её характеристиками
        foreach (var series in pumpSeriesList)
        {
            // Для каждой серии берем стандартные размеры, но генерируем УНИКАЛЬНЫЕ часы

            // 1. Наполняем колеса (Impeller)
            foreach (var size in standardImpellers)
            {
                context.PumpImpellers.Add(new PumpImpeller
                {
                    PumpSeriesId = series.Id,
                    Impeller = size,
                    // Случайные часы от 1.0 до 5.0 с округлением до 1 знака
                    Hour = Math.Round(random.NextDouble() * (5.0 - 1.0) + 1.0, 2)
                });
            }

            // 2. Наполняем вес (Weight)
            foreach (var weight in standardWeights)
            {
                context.PumpWeights.Add(new PumpWeight
                {
                    PumpSeriesId = series.Id,
                    Weight = weight,
                    Hour = Math.Round(random.NextDouble() * (6.0 - 2.0) + 2.0, 2)
                });
            }

            // 3. Наполняем объем (Volume)
            foreach (var volume in standardVolumes)
            {
                context.PumpVolumes.Add(new PumpVolume
                {
                    PumpSeriesId = series.Id,
                    Volume = volume,
                    Hour = Math.Round(random.NextDouble() * (3.0 - 0.5) + 0.5, 2)
                });
            }
        }

        // Сохраняем характеристики, чтобы они получили свои Id
        context.SaveChanges();

        // 4. Теперь создаем финальные сборки (PumpCentrifugalDesign)
        // Для каждой серии свяжем первое колесо, первый вес и первый объем в готовый дизайн
        foreach (var series in pumpSeriesList)
        {
            // Вытаскиваем только что созданные характеристики именно для этой серии
            var firstImpeller = context.PumpImpellers.First(i => i.PumpSeriesId == series.Id);
            var firstWeight = context.PumpWeights.First(w => w.PumpSeriesId == series.Id);
            var firstVolume = context.PumpVolumes.First(v => v.PumpSeriesId == series.Id);

            context.PumpCentrifugalDesigns.Add(new PumpCentrifugalDesign
            {
                PumpSeriesId = series.Id,
                PumpImpellerId = firstImpeller.Id,
                PumpWeightId = firstWeight.Id,
                PumpVolumeId = firstVolume.Id
            });
        }

        // Финальное сохранение сборок
        context.SaveChanges();
    }*/