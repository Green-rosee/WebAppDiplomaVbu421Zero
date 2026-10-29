namespace WebAppEstimate.Areas.SiteAdam.Services.ExcelPumps;

/// <summary>
///     Интерфейс сервиса импорта нормативных данных
///     центробежных насосов из Excel-файлов.
///     Определяет контракт для чтения Excel-файла
///     и сохранения нормативного набора в базу данных.
/// </summary>
public interface ICentrifugalPumpExcelImportService
{
    /// <summary>
    ///     Импортирует нормативные данные
    ///     из Excel-файла в базу данных.
    /// </summary>
    /// <param name="stream">
    ///     Поток с содержимым Excel-файла.
    /// </param>
    /// <param name="dataSetName">
    ///     Название нормативного набора,
    ///     указанное пользователем.
    /// </param>
    /// <param name="fileName">
    ///     Исходное имя Excel-файла.
    /// </param>
    /// <returns>
    ///     Асинхронная операция импорта данных.
    /// </returns>
    Task ImportAsync(
        Stream stream,
        string dataSetName,
        string fileName);
}

