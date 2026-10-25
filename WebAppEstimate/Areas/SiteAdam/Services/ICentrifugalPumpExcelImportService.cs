
namespace WebAppEstimate.Areas.SiteAdam.Services.ExcelPumps;

/// <summary>
/// Интерфейс сервиса импорта нормативных данных
/// центробежных насосов из Excel-файла.
/// </summary>
public interface ICentrifugalPumpExcelImportService
{
    /// <summary>
    /// Импортирует нормативные данные из Excel-файла
    /// в базу данных.
    /// </summary>
    /// <param name="stream">
    /// Поток загруженного Excel-файла.
    /// </param>
    /// <param name="dataSetName">
    /// Название нормативного набора, например A, B или C.
    /// </param>
    /// <param name="fileName">
    /// Исходное имя загруженного Excel-файла.
    /// </param>
    Task ImportAsync(
        Stream stream,
        string dataSetName,
        string fileName);
}