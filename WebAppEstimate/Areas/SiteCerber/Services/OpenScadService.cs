using System.Diagnostics;
using System.Globalization;
using WebAppEstimate.Areas.SiteCerber.Models;

namespace WebAppEstimate.Areas.SiteCerber.Services;

public class OpenScadService : IOpenScadService
{
    //создать в этом классе список коментариев под методами что они делают
    private readonly IWebHostEnvironment _environment;

    public OpenScadService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> GenerateFlangeAsync(FlangeModel model)
    {
        var fileName = $"flange_{Guid.NewGuid():N}";

        // Внутренние файлы OpenSCAD
        var outputDirectory = Path.Combine(
            _environment.ContentRootPath,
            "Areas",
            "SiteCerber",
            "OpenScad",
            "Generated");

        Directory.CreateDirectory(outputDirectory);

        var scadPath = Path.Combine(
            outputDirectory,
            $"{fileName}.scad");

        var stlPath = Path.Combine(
            outputDirectory,
            $"{fileName}.stl");

        // Публичная папка для preview
        var publicDirectory = Path.Combine(
            _environment.WebRootPath,
            "generated",
            "sitecerber");

        Directory.CreateDirectory(publicDirectory);

        var previewPath = Path.Combine(
            publicDirectory,
            $"{fileName}.png");
        //------------------
        var publicStlPath = Path.Combine(
            publicDirectory,
            $"{fileName}.stl");

        /*File.Copy(
            stlPath,
            publicStlPath,
            overwrite: true);*/
        //------------------

        var culture = CultureInfo.InvariantCulture;

        var boltCircleRadius =
            model.BoltCircleDiameter / 2.0;
        //--------------------

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

        await File.WriteAllTextAsync(
            scadPath,
            scad);

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
            throw new InvalidOperationException(
                "Не удалось запустить OpenSCAD.");

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            var error =
                await process.StandardError.ReadToEndAsync();

            throw new InvalidOperationException(
                $"OpenSCAD error: {error}");
        }

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

        previewProcessInfo.ArgumentList.Add("--imgsize=1000,800");

        previewProcessInfo.ArgumentList.Add(scadPath);

        using var previewProcess =
            Process.Start(previewProcessInfo);

        if (previewProcess == null)
            throw new InvalidOperationException(
                "Не удалось создать предпросмотр OpenSCAD.");

        await previewProcess.WaitForExitAsync();

        if (previewProcess.ExitCode != 0)
        {
            var error =
                await previewProcess.StandardError.ReadToEndAsync();

            throw new InvalidOperationException(
                $"OpenSCAD preview error: {error}");
        }

        // STL уже создан — теперь публикуем его
        File.Copy(
            stlPath,
            publicStlPath,
            true);

        return $"/generated/sitecerber/{fileName}.stl";
        //return stlPath;
    }
}