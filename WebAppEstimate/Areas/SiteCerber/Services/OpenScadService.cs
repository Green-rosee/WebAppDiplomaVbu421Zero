using System.Diagnostics;
using System.Globalization;
using WebAppEstimate.Areas.SiteCerber.Models;

namespace WebAppEstimate.Areas.SiteCerber.Services;



/// <summary>
/// Сервис генерации параметрической 3D-модели фланца через OpenSCAD.
/// Создаёт SCAD-файл, STL-модель и PNG-предпросмотр.
/// </summary>
public class OpenScadService : IOpenScadService
{
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Создаёт сервис генерации OpenSCAD-моделей.
    /// </summary>
    /// <param name="environment">
    /// Информация о файловой структуре и корневых каталогах приложения.
    /// </param>
    public OpenScadService(
        IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    /// <summary>
    /// Генерирует параметрическую модель фланца
    /// на основе переданных геометрических параметров.
    /// </summary>
    /// <param name="model">
    /// Параметры фланца:
    /// наружный и внутренний диаметры, толщина,
    /// количество отверстий, их диаметр
    /// и диаметр болтовой окружности.
    /// </param>
    /// <returns>
    /// Публичный URL созданного STL-файла.
    /// </returns>
    public async Task<string> GenerateFlangeAsync(
        FlangeModel model)
    {
        // Формируем уникальное имя набора файлов,
        // чтобы несколько генераций не перезаписывали друг друга.
        var fileName =
            $"flange_{Guid.NewGuid():N}";

        // Внутренняя папка для служебных файлов OpenSCAD.
        var outputDirectory =
            Path.Combine(
                _environment.ContentRootPath,
                "Areas",
                "SiteCerber",
                "OpenScad",
                "Generated");

        Directory.CreateDirectory(
            outputDirectory);

        var scadPath =
            Path.Combine(
                outputDirectory,
                $"{fileName}.scad");

        var stlPath =
            Path.Combine(
                outputDirectory,
                $"{fileName}.stl");

        // Публичная папка wwwroot для файлов,
        // которые должны быть доступны из браузера.
        var publicDirectory =
            Path.Combine(
                _environment.WebRootPath,
                "generated",
                "sitecerber");

        Directory.CreateDirectory(
            publicDirectory);

        var previewPath =
            Path.Combine(
                publicDirectory,
                $"{fileName}.png");

        var publicStlPath =
            Path.Combine(
                publicDirectory,
                $"{fileName}.stl");

        // Используем InvariantCulture,
        // чтобы десятичные значения в SCAD записывались через точку.
        var culture =
            CultureInfo.InvariantCulture;

        // OpenSCAD использует радиус болтовой окружности,
        // поэтому диаметр преобразуется в радиус.
        var boltCircleRadius =
            model.BoltCircleDiameter / 2.0;

        // Формируем параметрический OpenSCAD-код.
        // Основной цилиндр образует тело фланца,
        // а внутренний цилиндр и отверстия вычитаются через difference().
        var scad = $$"""
                     difference()
                     {
                         cylinder(
                             h = {{model.Thickness.ToString(culture)}},
                             d = {{model.OuterDiameter.ToString(culture)}},
                             $fn = 100
                         );

                         translate([0, 0, -1])
                         cylinder(
                             h = {{(model.Thickness + 2).ToString(culture)}},
                             d = {{model.InnerDiameter.ToString(culture)}},
                             $fn = 100
                         );

                         for (i = [0 : {{model.HoleCount - 1}}])
                         {
                             rotate([
                                 0,
                                 0,
                                 i * 360 / {{model.HoleCount}}
                             ])
                             translate([
                                 {{boltCircleRadius.ToString(culture)}},
                                 0,
                                 -1
                             ])
                             cylinder(
                                 h = {{(model.Thickness + 2).ToString(culture)}},
                                 d = {{model.HoleDiameter.ToString(culture)}},
                                 $fn = 50
                             );
                         }
                     }
                     """;

        // Сохраняем сформированный OpenSCAD-скрипт на диск.
        await File.WriteAllTextAsync(
            scadPath,
            scad);

        // Запускаем OpenSCAD для генерации STL-файла.
        var processInfo =
            new ProcessStartInfo
            {
                FileName = "openscad",
                Arguments =
                    $"-o \"{stlPath}\" \"{scadPath}\"",

                RedirectStandardOutput = true,
                RedirectStandardError = true,

                UseShellExecute = false,
                CreateNoWindow = true
            };

        using var process =
            Process.Start(processInfo);

        if (process == null)
        {
            throw new InvalidOperationException(
                "Не удалось запустить OpenSCAD.");
        }

        await process.WaitForExitAsync();

        // Если OpenSCAD завершился с ошибкой,
        // получаем stderr и передаём его в исключение.
        if (process.ExitCode != 0)
        {
            var error =
                await process.StandardError
                    .ReadToEndAsync();

            throw new InvalidOperationException(
                $"OpenSCAD error: {error}");
        }

        // Создаём PNG-предпросмотр модели.
        // xvfb-run позволяет запускать графический OpenSCAD
        // в контейнере без физического дисплея.
        var previewProcessInfo =
            new ProcessStartInfo
            {
                FileName = "xvfb-run",

                RedirectStandardOutput = true,
                RedirectStandardError = true,

                UseShellExecute = false,
                CreateNoWindow = true
            };

        previewProcessInfo.ArgumentList.Add("-a");
        previewProcessInfo.ArgumentList.Add("openscad");

        previewProcessInfo.ArgumentList.Add("-o");
        previewProcessInfo.ArgumentList.Add(previewPath);

        previewProcessInfo.ArgumentList.Add(
            "--imgsize=1000,800");

        previewProcessInfo.ArgumentList.Add(
            scadPath);

        using var previewProcess =
            Process.Start(
                previewProcessInfo);

        if (previewProcess == null)
        {
            throw new InvalidOperationException(
                "Не удалось создать предпросмотр OpenSCAD.");
        }

        await previewProcess.WaitForExitAsync();

        if (previewProcess.ExitCode != 0)
        {
            var error =
                await previewProcess.StandardError
                    .ReadToEndAsync();

            throw new InvalidOperationException(
                $"OpenSCAD preview error: {error}");
        }

        // Публикуем готовый STL в wwwroot,
        // чтобы его можно было скачать через браузер.
        File.Copy(
            stlPath,
            publicStlPath,
            true);

        // Возвращаем URL публичного STL-файла.
        return
            $"/generated/sitecerber/{fileName}.stl";
    }
}











