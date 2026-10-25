
namespace WebAppEstimate.Areas.SiteAdam.Models;

/// <summary>
/// Представляет отдельный набор нормативных данных,
/// загруженный из Excel-файла.
/// </summary>
public class PumpNormativeSet
{
    /// <summary>
    /// Уникальный идентификатор набора нормативов.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Отображаемое название набора,
    /// например "Норматив A", "Норматив B" или "Норматив C".
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Имя исходного Excel-файла,
    /// из которого были загружены нормативные данные.
    /// </summary>
    public string FileName { get; set; } = "";

    /// <summary>
    /// Дата и время загрузки нормативного набора.
    /// </summary>
    public DateTime CreatedAt { get; set; } =
        DateTime.UtcNow;
}