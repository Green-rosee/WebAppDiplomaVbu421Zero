
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAppEstimate.Areas.SiteAdam.Services.ExcelPumps;

namespace WebAppEstimate.Areas.SiteAdam.Controllers;

/// <summary>
/// Контроллер загрузки нормативных данных центробежных насосов
/// из Excel-файлов в базу данных.
/// </summary>
[Area("SiteAdam")]
[Authorize(Roles = "Admin,Adam")]
public class CentrifugalPumpImportController : Controller
{
    private readonly ICentrifugalPumpExcelImportService _importService;

    /// <summary>
    /// Создаёт контроллер импорта Excel-файлов.
    /// </summary>
    /// <param name="importService">
    /// Сервис чтения Excel и сохранения нормативных данных в БД.
    /// </param>
    public CentrifugalPumpImportController(
        ICentrifugalPumpExcelImportService importService)
    {
        _importService = importService;
    }

    /// <summary>
    /// Отображает страницу загрузки Excel-справочников.
    /// </summary>
    /// <summary>
    /// Принимает Excel-файл и передаёт его
    /// сервису импорта нормативных данных.
    /// </summary>
    /// <param name="file">
    /// Загруженный пользователем Excel-файл.
    /// </param>
    /// <param name="dataSetName">
    /// Название нормативного набора, например A, B или C.
    /// </param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(
        IFormFile file,
        string dataSetName)
    {
        // Проверяем, что пользователь выбрал файл.
        if (file == null || file.Length == 0)
        {
            TempData["ErrorMessage"] =
                "Выберите Excel-файл.";

            return RedirectToAction(nameof(Index));
        }

        // Проверяем название нормативного набора.
        if (string.IsNullOrWhiteSpace(dataSetName))
        {
            TempData["ErrorMessage"] =
                "Укажите название набора нормативов.";

            return RedirectToAction(nameof(Index));
        }

        // Разрешаем загружать только Excel-файлы.
        var extension =
            Path.GetExtension(file.FileName)
                .ToLowerInvariant();

        if (extension != ".xlsx" &&
            extension != ".xls")
        {
            TempData["ErrorMessage"] =
                "Допустимы только файлы .xlsx и .xls.";

            return RedirectToAction(nameof(Index));
        }

        try
        {
            // Получаем поток файла без сохранения
            // промежуточной копии на диске сервера.
            await using var stream =
                file.OpenReadStream();

            // Передаём поток ExcelDataReader-сервису.
            await _importService.ImportAsync(
                stream,
                dataSetName,
                file.FileName);

            TempData["SuccessMessage"] =
                $"Набор нормативов '{dataSetName}' " +
                $"из файла '{file.FileName}' успешно загружен.";
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] =
                $"Ошибка импорта Excel: {e.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        var folder =
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "ExcelFilesPageLoad",
                "CentrPumps");

        var files =
            Directory.Exists(folder)
                ? Directory.GetFiles(folder, "*.xlsx")
                    .Select(Path.GetFileName)
                    .Where(x => x != null)
                    .ToList()
                : new List<string?>();

        ViewBag.ExcelFiles = files;

        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadFromServer(
        string fileName,
        string dataSetName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            TempData["ErrorMessage"] =
                "Выберите Excel-файл.";

            return RedirectToAction(nameof(Index));
        }

        if (string.IsNullOrWhiteSpace(dataSetName))
        {
            TempData["ErrorMessage"] =
                "Укажите название набора нормативов.";

            return RedirectToAction(nameof(Index));
        }

        var folder =
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "ExcelFilesPageLoad",
                "CentrPumps");

        var fullPath =
            Path.Combine(folder, fileName);

        if (!System.IO.File.Exists(fullPath))
        {
            TempData["ErrorMessage"] =
                "Файл не найден на сервере.";

            return RedirectToAction(nameof(Index));
        }

        try
        {
            await using var stream =
                System.IO.File.OpenRead(fullPath);

            await _importService.ImportAsync(
                stream,
                dataSetName,
                fileName);

            TempData["SuccessMessage"] =
                $"Набор нормативов '{dataSetName}' " +
                $"из файла '{fileName}' успешно загружен.";
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] =
                $"Ошибка импорта Excel: {e.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}