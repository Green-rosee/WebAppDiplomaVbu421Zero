using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebAppEstimate.Areas.SiteAdam.Models;

public class CentrifugalPumpModel
{
    //цена за час работы 
    public decimal CostHour { get; set; }

    // НАЗВАНИЕ НАСОСА ОНО НЕ ОТНОСИТЬСЯ К СЕРИИ... СЕРИЯ ЭТО ТОКА КОНСТРУКТ. ОСОБЕНОСТЬ
    public string Name { get; set; } = string.Empty;

    // ИСПРАВЛЕНИЕ: Храним int Id выбранной серии для точной связи по Foreign Key
    public int SelectedSeriesId { get; set; }

    // ИСПРАВЛЕНИЕ: Тип изменен на int, чтобы соответствовать сущности PumpImpeller в БД
    public int Impeller { get; set; }
    public double HourImpeller { get; set; }

    // ИСПРАВЛЕНИЕ: Тип изменен на int, чтобы соответствовать сущности PumpWeight в БД
    public int Weight { get; set; }
    public double HourWeight { get; set; }

    // ИСПРАВЛЕНИЕ: Тип изменен на int, чтобы соответствовать сущности PumpVolume в БД
    public int Volume { get; set; }
    public double HourVolume { get; set; }

    //КОЛ-ВО  насосов в  форме 
    public int Number { get; set; }

    // НОВОЕ ПОЛЕ: Итоговая сумма часов за все насосы
    public double TotalSumHour { get; set; }

    //сумма за все часы
    public decimal TotalSumCostHour { get; set; }


    // Флаг, чтобы Razor View понимал, что расчет успешно выполнен
    public bool IsCalculated { get; set; }

    // Сюда мы будем загружать список серий динамически из вашей базы данных
    public List<SelectListItem> SeriesOptions { get; set; } = new();
}