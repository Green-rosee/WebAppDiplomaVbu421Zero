using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppEstimate.Areas.SiteAdam.Models;

namespace WebAppEstimate.Areas.SiteAdam.Services;

/// <summary>
///     Интерфейс сервиса расчёта центробежных насосов.
///     Определяет операции получения справочных данных
///     и выполнения расчёта трудоёмкости
///     на основе выбранной серии и нормативного набора.
/// </summary>
public interface ICentrifugalPumpService
{
    /// <summary>
    ///     Получает список доступных серий насосов
    ///     для отображения в выпадающем списке интерфейса.
    /// </summary>
    /// <returns>
    ///     Список элементов <see cref="SelectListItem" />
    ///     с идентификаторами и названиями серий.
    /// </returns>
    Task<List<SelectListItem>>
        GetSeriesOptionsAsync();

    /// <summary>
    ///     Выполняет расчёт трудоёмкости
    ///     и общей стоимости изготовления насоса.
    ///     Для введённых параметров подбираются
    ///     ближайшие нормативные значения
    ///     в рамках выбранной серии
    ///     и нормативного набора.
    /// </summary>
    /// <param name="model">
    ///     Модель с исходными параметрами насоса,
    ///     выбранной серией и нормативным набором.
    /// </param>
    /// <returns>
    ///     Обновлённая модель
    ///     с рассчитанными значениями трудоёмкости
    ///     и итоговой стоимости.
    /// </returns>
    Task<CentrifugalPumpModel>
        CalculateClosestHoursAsync(
            CentrifugalPumpModel model);

    /// <summary>
    ///     Получает список загруженных нормативных наборов
    ///     для отображения в выпадающем списке
    ///     на странице расчёта.
    /// </summary>
    /// <returns>
    ///     Список нормативных наборов
    ///     с их идентификаторами и названиями.
    /// </returns>
    Task<List<SelectListItem>>
        GetNormativeSetOptionsAsync();
}

