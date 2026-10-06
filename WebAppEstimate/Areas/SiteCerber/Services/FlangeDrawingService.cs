using System.Globalization;
using System.Text;
using WebAppEstimate.Areas.SiteCerber.Models;

namespace WebAppEstimate.Areas.SiteCerber.Services;

public class FlangeDrawingService : IFlangeDrawingService
{
    private readonly IWebHostEnvironment _environment;

    public FlangeDrawingService(
        IWebHostEnvironment environment)
    {
        _environment = environment;
    }


    public async Task<string> GenerateSvgAsync(
        FlangeModel model)
    {
        var publicDirectory = Path.Combine(
            _environment.WebRootPath,
            "generated",
            "sitecerber");

        Directory.CreateDirectory(publicDirectory);

        var fileName =
            $"flange_drawing_{Guid.NewGuid():N}.svg";

        var filePath = Path.Combine(
            publicDirectory,
            fileName);

        var culture = CultureInfo.InvariantCulture;

        //-------редактирование вида
        const double canvasWidth = 1000;
        const double canvasHeight = 1200;

        const double centerX = 500;
        const double centerY = 430;

        var scale =
            520.0 / model.OuterDiameter;

        var outerRadius =
            model.OuterDiameter / 2.0 * scale;

        var innerRadius =
            model.InnerDiameter / 2.0 * scale;

        var boltCircleRadius =
            model.BoltCircleDiameter / 2.0 * scale;

        var holeRadius =
            model.HoleDiameter / 2.0 * scale;

        var svg = new StringBuilder();

        svg.AppendLine(
            $$"""
              <svg xmlns="http://www.w3.org/2000/svg"
                   width="{{canvasWidth}}"
                   height="{{canvasHeight}}"
                   viewBox="0 0 {{canvasWidth}} {{canvasHeight}}">
              """);

        // -------------------------
        // Стрелки
        // -------------------------
        svg.AppendLine(
            """
            <defs>
                <marker id="arrow"
                        markerWidth="10"
                        markerHeight="10"
                        refX="5"
                        refY="5"
                        orient="auto"
                        markerUnits="strokeWidth">
                    <path d="M0,0 L10,5 L0,10 z"
                          fill="black" />
                </marker>
            </defs>
            """);

        // -------------------------
        // Фон
        // -------------------------
        svg.AppendLine(
            """
            <rect width="100%"
                  height="100%"
                  fill="white"/>
            """);

        // -------------------------
        // Заголовок
        // -------------------------
        AppendText(
            svg,
            40,
            40,
            "Чертёж фланца",
            26,
            "start");

        AppendText(
            svg,
            40,
            75,
            $"Наружный диаметр: ⌀{model.OuterDiameter:0}",
            18,
            "start");

        AppendText(
            svg,
            40,
            105,
            $"Внутренний диаметр: ⌀{model.InnerDiameter:0}",
            18,
            "start");

        AppendText(
            svg,
            40,
            135,
            $"Отверстия: {model.HoleCount} × ⌀{model.HoleDiameter:0}",
            18,
            "start");

        AppendText(
            svg,
            40,
            165,
            $"Болтовая окружность: ⌀{model.BoltCircleDiameter:0}",
            18,
            "start");

        AppendText(
            svg,
            40,
            195,
            $"Толщина: {model.Thickness:0} мм",
            18,
            "start");

        // -------------------------
        // Наружная окружность
        // -------------------------
        AppendCircle(
            svg,
            centerX,
            centerY,
            outerRadius,
            "black",
            2.5);

        // -------------------------
        // Центральное отверстие
        // -------------------------
        AppendCircle(
            svg,
            centerX,
            centerY,
            innerRadius,
            "black",
            2.5);

        // -------------------------
        // Болтовая окружность
        // -------------------------
        AppendCircle(
            svg,
            centerX,
            centerY,
            boltCircleRadius,
            "gray",
            1.2,
            "none",
            "8,6");

        // -------------------------
        // Отверстия
        // -------------------------
        for (var i = 0; i < model.HoleCount; i++)
        {
            var angle =
                2.0 * Math.PI * i / model.HoleCount;

            var x =
                centerX +
                boltCircleRadius * Math.Cos(angle);

            var y =
                centerY +
                boltCircleRadius * Math.Sin(angle);

            AppendCircle(
                svg,
                x,
                y,
                holeRadius,
                "black",
                1.8);
        }

        // -------------------------
        // Центровые линии
        // -------------------------
        AppendLine(
            svg,
            centerX - outerRadius - 50,
            centerY,
            centerX + outerRadius + 50,
            centerY,
            "gray",
            1,
            "10,5");

        AppendLine(
            svg,
            centerX,
            centerY - outerRadius - 50,
            centerX,
            centerY + outerRadius + 50,
            "gray",
            1,
            "10,5");

        // -------------------------
        // Наружный диаметр
        // -------------------------
        var outerDimY =
            centerY + outerRadius + 70;

        AppendLine(
            svg,
            centerX - outerRadius,
            centerY,
            centerX - outerRadius,
            outerDimY - 10);

        AppendLine(
            svg,
            centerX + outerRadius,
            centerY,
            centerX + outerRadius,
            outerDimY - 10);

        AppendLine(
            svg,
            centerX - outerRadius,
            outerDimY,
            centerX + outerRadius,
            outerDimY,
            "black",
            1.3,
            null,
            "arrow",
            "arrow");

        AppendText(
            svg,
            centerX,
            outerDimY - 10,
            $"⌀{model.OuterDiameter:0}",
            16);

        // -------------------------
        // Внутренний диаметр
        // -------------------------
        var innerDimY =
            centerY - innerRadius - 45;

        AppendLine(
            svg,
            centerX - innerRadius,
            centerY,
            centerX - innerRadius,
            innerDimY + 10);

        AppendLine(
            svg,
            centerX + innerRadius,
            centerY,
            centerX + innerRadius,
            innerDimY + 10);

        AppendLine(
            svg,
            centerX - innerRadius,
            innerDimY,
            centerX + innerRadius,
            innerDimY,
            "black",
            1.3,
            null,
            "arrow",
            "arrow");

        AppendText(
            svg,
            centerX,
            innerDimY - 10,
            $"⌀{model.InnerDiameter:0}",
            16);

        // -------------------------
        // Болтовая окружность
        // -------------------------
        var boltDimY =
            centerY + boltCircleRadius + 35;

        AppendLine(
            svg,
            centerX - boltCircleRadius,
            centerY,
            centerX - boltCircleRadius,
            boltDimY - 10);

        AppendLine(
            svg,
            centerX + boltCircleRadius,
            centerY,
            centerX + boltCircleRadius,
            boltDimY - 10);

        AppendLine(
            svg,
            centerX - boltCircleRadius,
            boltDimY,
            centerX + boltCircleRadius,
            boltDimY,
            "black",
            1.1,
            "4,4",
            "arrow",
            "arrow");

        AppendText(
            svg,
            centerX,
            boltDimY - 8,
            $"PCD ⌀{model.BoltCircleDiameter:0}",
            15);

        // -------------------------
        // Выноска на отверстия
        // -------------------------
        if (model.HoleCount > 0)
        {
            var angle =
                -Math.PI / 2.0;

            var holeX =
                centerX +
                boltCircleRadius * Math.Cos(angle);

            var holeY =
                centerY +
                boltCircleRadius * Math.Sin(angle);

            var leaderX =
                holeX + 120;

            var leaderY =
                holeY - 70;

            AppendLine(
                svg,
                holeX,
                holeY,
                leaderX,
                leaderY,
                "black",
                1.2);

            AppendLine(
                svg,
                leaderX,
                leaderY,
                leaderX + 120,
                leaderY,
                "black",
                1.2);

            AppendText(
                svg,
                leaderX + 130,
                leaderY - 5,
                $"{model.HoleCount} × ⌀{model.HoleDiameter:0}",
                16,
                "start");
        }
        
        // -------------------------
        // Разделитель
        // -------------------------
        AppendLine(
            svg,
            60,
            780,
            940,
            780,
            "gray",
            1,
            "6,6");

        // -------------------------
        //-------Боковой вид
        // -------------------------
        var sideX =
            250.0;

        var sideY =
            1050.0;

        var sideWidth =
            model.OuterDiameter * scale * 0.8;

        var sideHeight =
            Math.Max(
                model.Thickness * scale,
                25);

        //------редактирую 
        AppendText(
            svg,
            sideX,
            sideY - sideHeight - 70,
            "Боковой вид",
            18,
            "start");

        svg.AppendLine(
            $"""
             <rect x="{sideX.ToString(culture)}"
                   y="{(sideY - sideHeight).ToString(culture)}"
                   width="{sideWidth.ToString(culture)}"
                   height="{sideHeight.ToString(culture)}"
                   fill="none"
                   stroke="black"
                   stroke-width="2"/>
             """);

        // Размер толщины
        var thicknessDimX =
            sideX + sideWidth + 50;

        AppendLine(
            svg,
            sideX + sideWidth,
            sideY - sideHeight,
            thicknessDimX - 10,
            sideY - sideHeight);

        AppendLine(
            svg,
            sideX + sideWidth,
            sideY,
            thicknessDimX - 10,
            sideY);

        AppendLine(
            svg,
            thicknessDimX,
            sideY - sideHeight,
            thicknessDimX,
            sideY,
            "black",
            1.2,
            null,
            "arrow",
            "arrow");

        AppendText(
            svg,
            thicknessDimX + 15,
            sideY - sideHeight / 2,
            $"{model.Thickness:0} мм",
            15,
            "start");

        // -------------------------
        // Закрываем SVG
        // -------------------------
        svg.AppendLine("</svg>");

        await File.WriteAllTextAsync(
            filePath,
            svg.ToString());

        return $"/generated/sitecerber/{fileName}";
    }

    private static void AppendLine(
        StringBuilder svg,
        double x1,
        double y1,
        double x2,
        double y2,
        string stroke = "black",
        double strokeWidth = 1,
        string? dashArray = null,
        string? markerStart = null,
        string? markerEnd = null)
    {
        var dash =
            string.IsNullOrWhiteSpace(dashArray)
                ? ""
                : $" stroke-dasharray=\"{dashArray}\"";

        var mStart =
            string.IsNullOrWhiteSpace(markerStart)
                ? ""
                : $" marker-start=\"url(#{markerStart})\"";

        var mEnd =
            string.IsNullOrWhiteSpace(markerEnd)
                ? ""
                : $" marker-end=\"url(#{markerEnd})\"";

        svg.AppendLine(
            $"<line x1=\"{x1:F2}\" " +
            $"y1=\"{y1:F2}\" " +
            $"x2=\"{x2:F2}\" " +
            $"y2=\"{y2:F2}\" " +
            $"stroke=\"{stroke}\" " +
            $"stroke-width=\"{strokeWidth}\"" +
            $"{dash}{mStart}{mEnd} />");
    }

    private static void AppendText(
        StringBuilder svg,
        double x,
        double y,
        string text,
        int fontSize = 14,
        string anchor = "middle",
        string fill = "black")
    {
        svg.AppendLine(
            $"<text x=\"{x:F2}\" " +
            $"y=\"{y:F2}\" " +
            $"font-family=\"Arial\" " +
            $"font-size=\"{fontSize}\" " +
            $"text-anchor=\"{anchor}\" " +
            $"fill=\"{fill}\">" +
            $"{text}" +
            $"</text>");
    }

    private static void AppendCircle(
        StringBuilder svg,
        double cx,
        double cy,
        double r,
        string stroke = "black",
        double strokeWidth = 1,
        string fill = "none",
        string? dashArray = null)
    {
        var dash =
            string.IsNullOrWhiteSpace(dashArray)
                ? ""
                : $" stroke-dasharray=\"{dashArray}\"";

        svg.AppendLine(
            $"<circle cx=\"{cx:F2}\" " +
            $"cy=\"{cy:F2}\" " +
            $"r=\"{r:F2}\" " +
            $"stroke=\"{stroke}\" " +
            $"stroke-width=\"{strokeWidth}\" " +
            $"fill=\"{fill}\"" +
            $"{dash} />");
    }
}

/*public async Task<string> GenerateSvgAsync(FlangeModel model)
{
    Console.WriteLine("=== GenerateSvgAsync START ===");


    var publicDirectory = Path.Combine(
        _environment.WebRootPath,
        "generated",
        "sitecerber");

    Directory.CreateDirectory(publicDirectory);

    var fileName =
        $"flange_drawing_{Guid.NewGuid():N}.svg";

    var filePath = Path.Combine(
        publicDirectory,
        fileName);

    var culture = CultureInfo.InvariantCulture;

    const double canvasSize = 800;
    const double center = 400;

    // Масштабируем деталь, чтобы она помещалась в SVG
    var scale =
        500.0 / model.OuterDiameter;

    var outerRadius =
        model.OuterDiameter / 2.0 * scale;

    var innerRadius =
        model.InnerDiameter / 2.0 * scale;

    var boltCircleRadius =
        model.BoltCircleDiameter / 2.0 * scale;

    var holeRadius =
        model.HoleDiameter / 2.0 * scale;

    var svg = new StringBuilder();

    svg.AppendLine(
        $$"""
        <svg xmlns="http://www.w3.org/2000/svg"
             width="{{canvasSize}}"
             height="{{canvasSize}}"
             viewBox="0 0 {{canvasSize}} {{canvasSize}}">
        """);

    svg.AppendLine(
        """
        <rect width="100%"
              height="100%"
              fill="white"/>
        """);

    // Наружная окружность
    svg.AppendLine(
        $$"""
        <circle cx="{{center}}"
                cy="{{center}}"
                r="{{outerRadius.ToString(culture)}}"
                fill="none"
                stroke="black"
                stroke-width="3"/>
        """);

    // Центральное отверстие
    svg.AppendLine(
        $$"""
        <circle cx="{{center}}"
                cy="{{center}}"
                r="{{innerRadius.ToString(culture)}}"
                fill="none"
                stroke="black"
                stroke-width="3"/>
        """);

    // Болтовая окружность
    svg.AppendLine(
        $$"""
        <circle cx="{{center}}"
                cy="{{center}}"
                r="{{boltCircleRadius.ToString(culture)}}"
                fill="none"
                stroke="gray"
                stroke-width="1"
                stroke-dasharray="8 8"/>
        """);

    // Отверстия
    for (var i = 0; i < model.HoleCount; i++)
    {
        var angle =
            2.0 * Math.PI * i / model.HoleCount;

        var x =
            center +
            boltCircleRadius * Math.Cos(angle);

        var y =
            center +
            boltCircleRadius * Math.Sin(angle);

        svg.AppendLine(
            $$"""
            <circle cx="{{x.ToString(culture)}}"
                    cy="{{y.ToString(culture)}}"
                    r="{{holeRadius.ToString(culture)}}"
                    fill="none"
                    stroke="black"
                    stroke-width="2"/>
            """);
    }

    // Центровые линии
    svg.AppendLine(
        $$"""
        <line x1="{{center - outerRadius - 30}}"
              y1="{{center}}"
              x2="{{center + outerRadius + 30}}"
              y2="{{center}}"
              stroke="gray"
              stroke-width="1"
              stroke-dasharray="10 5"/>

        <line x1="{{center}}"
              y1="{{center - outerRadius - 30}}"
              x2="{{center}}"
              y2="{{center + outerRadius + 30}}"
              stroke="gray"
              stroke-width="1"
              stroke-dasharray="10 5"/>
        """);

    // Текст размеров
    svg.AppendLine(
        $$"""
        <text x="40"
              y="50"
              font-family="Arial"
              font-size="22">
            Наружный диаметр: Ø{{model.OuterDiameter}}
        </text>

        <text x="40"
              y="80"
              font-family="Arial"
              font-size="22">
            Внутренний диаметр: Ø{{model.InnerDiameter}}
        </text>

        <text x="40"
              y="110"
              font-family="Arial"
              font-size="22">
            Отверстия: {{model.HoleCount}} × Ø{{model.HoleDiameter}}
        </text>

        <text x="40"
              y="140"
              font-family="Arial"
              font-size="22">
            Болтовая окружность: Ø{{model.BoltCircleDiameter}}
        </text>

        <text x="40"
              y="170"
              font-family="Arial"
              font-size="22">
            Толщина: {{model.Thickness}} мм
        </text>
        """);

    svg.AppendLine("</svg>");

    Console.WriteLine($"SVG filePath = {filePath}");

    await File.WriteAllTextAsync(
        filePath,
        svg.ToString());

    Console.WriteLine($"SVG created = {File.Exists(filePath)}");

    return $"/generated/sitecerber/{fileName}";
}*/