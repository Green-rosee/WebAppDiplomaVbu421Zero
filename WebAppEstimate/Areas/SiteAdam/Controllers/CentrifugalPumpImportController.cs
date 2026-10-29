using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAppEstimate.Areas.SiteAdam.Services.ExcelPumps;

namespace WebAppEstimate.Areas.SiteAdam.Controllers;

/// <summary>
///     Контроллер импорта нормативных данных центробежных насосов
///     из Excel-файлов в базу данных.
///     Поддерживает два варианта загрузки:
///     файл с компьютера пользователя
///     и файл из заранее подготовленной папки на сервере.
/// </summary>
[Area("SiteAdam")]
[Authorize(Roles = "Admin,Adam")]
public class CentrifugalPumpImportController : Controller
{
    private readonly ICentrifugalPumpExcelImportService _importService;

    /// <summary>
    ///     Создаёт контроллер импорта нормативных Excel-файлов.
    /// </summary>
    /// <param name="importService">
    ///     Сервис чтения Excel-файлов
    ///     и сохранения нормативных данных в базу.
    /// </param>
    public CentrifugalPumpImportController(
        ICentrifugalPumpExcelImportService importService)
    {
        _importService = importService;
    }

    /// <summary>
    ///     Принимает Excel-файл, загруженный пользователем,
    ///     проверяет его и передаёт сервису импорта
    ///     нормативных данных.
    /// </summary>
    /// <param name="file">
    ///     Excel-файл, выбранный пользователем
    ///     на локальном компьютере.
    /// </param>
    /// <param name="dataSetName">
    ///     Название создаваемого нормативного набора.
    /// </param>
    /// <returns>
    ///     Перенаправление обратно на страницу импорта
    ///     с сообщением об успешной загрузке или ошибке.
    /// </returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(
        IFormFile file,
        string dataSetName)
    {
        // Проверяем, что пользователь выбрал файл.
        if (file == null ||
            file.Length == 0)
        {
            TempData["ErrorMessage"] =
                "Выберите Excel-файл.";

            return RedirectToAction(
                nameof(Index));
        }

        // Проверяем, что пользователь указал
        // название нормативного набора.
        if (string.IsNullOrWhiteSpace(dataSetName))
        {
            TempData["ErrorMessage"] =
                "Укажите название набора нормативов.";

            return RedirectToAction(
                nameof(Index));
        }

        // Разрешаем загрузку только Excel-файлов
        // форматов .xlsx и .xls.
        var extension =
            Path.GetExtension(
                    file.FileName)
                .ToLowerInvariant();

        if (extension != ".xlsx" &&
            extension != ".xls")
        {
            TempData["ErrorMessage"] =
                "Допустимы только файлы .xlsx и .xls.";

            return RedirectToAction(
                nameof(Index));
        }

        try
        {
            // Получаем поток загруженного файла
            // без создания промежуточной копии
            // на диске сервера.
            await using var stream =
                file.OpenReadStream();

            // Передаём поток сервису импорта,
            // который читает Excel и сохраняет
            // нормативные данные в базу.
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

        return RedirectToAction(
            nameof(Index));
    }

    /// <summary>
    ///     Отображает страницу импорта нормативных данных
    ///     и формирует список Excel-файлов,
    ///     доступных в серверной папке приложения.
    /// </summary>
    /// <returns>
    ///     Представление страницы импорта Excel-файлов.
    /// </returns>
    [HttpGet]
    public IActionResult Index()
    {
        var folder =
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "ExcelFilesPageLoad",
                "CentrPumps");

        // Получаем список Excel-файлов,
        // находящихся в серверной папке.
        var files =
            Directory.Exists(folder)
                ? Directory
                    .GetFiles(
                        folder,
                        "*.xlsx")
                    .Select(
                        Path.GetFileName)
                    .Where(x => x != null)
                    .ToList()
                : new List<string?>();

        // Передаём список файлов в представление,
        // чтобы пользователь мог выбрать
        // нормативный справочник на сервере.
        ViewBag.ExcelFiles =
            files;

        return View();
    }

    /// <summary>
    ///     Импортирует нормативный набор
    ///     из Excel-файла, уже находящегося
    ///     в серверной папке приложения.
    /// </summary>
    /// <param name="fileName">
    ///     Имя Excel-файла в серверной папке.
    /// </param>
    /// <param name="dataSetName">
    ///     Название создаваемого нормативного набора.
    /// </param>
    /// <returns>
    ///     Перенаправление обратно на страницу импорта
    ///     с сообщением об успешной загрузке или ошибке.
    /// </returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadFromServer(
        string fileName,
        string dataSetName)
    {
        // Проверяем, что пользователь выбрал
        // файл из серверного списка.
        if (string.IsNullOrWhiteSpace(fileName))
        {
            TempData["ErrorMessage"] =
                "Выберите Excel-файл.";

            return RedirectToAction(
                nameof(Index));
        }

        // Проверяем название нормативного набора.
        if (string.IsNullOrWhiteSpace(dataSetName))
        {
            TempData["ErrorMessage"] =
                "Укажите название набора нормативов.";

            return RedirectToAction(
                nameof(Index));
        }

        // Формируем путь к папке,
        // где хранятся исходные нормативные Excel-файлы.
        var folder =
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "ExcelFilesPageLoad",
                "CentrPumps");

        var fullPath =
            Path.Combine(
                folder,
                fileName);

        // Проверяем наличие выбранного файла
        // на сервере.
        if (!System.IO.File.Exists(fullPath))
        {
            TempData["ErrorMessage"] =
                "Файл не найден на сервере.";

            return RedirectToAction(
                nameof(Index));
        }

        try
        {
            // Открываем серверный Excel-файл
            // только для чтения.
            await using var stream =
                System.IO.File.OpenRead(
                    fullPath);

            // Передаём файл сервису импорта.
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

        return RedirectToAction(
            nameof(Index));
    }
}

