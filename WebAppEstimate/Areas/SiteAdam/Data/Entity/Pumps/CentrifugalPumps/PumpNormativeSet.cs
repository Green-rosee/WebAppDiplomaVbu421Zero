namespace WebAppEstimate.Areas.SiteAdam.Models;

/// <summary>
///     Представляет отдельный набор нормативных данных,
///     загруженный из Excel-файла.
///     Используется для разделения нескольких
///     нормативных справочников, например A, B и C.
/// </summary>
public class PumpNormativeSet
{
    /// <summary>
    ///     Уникальный идентификатор
    ///     нормативного набора.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Отображаемое название набора нормативов.
    ///     Например: "Норматив A",
    ///     "Норматив B" или "Норматив C".
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    ///     Имя исходного Excel-файла,
    ///     из которого были импортированы
    ///     нормативные данные.
    /// </summary>
    public string FileName { get; set; } = "";

    /// <summary>
    ///     Дата и время создания
    ///     нормативного набора в базе данных.
    ///     Значение сохраняется в формате UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; } =
        DateTime.UtcNow;
}

