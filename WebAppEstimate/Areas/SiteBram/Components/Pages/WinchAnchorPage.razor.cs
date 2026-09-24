using Microsoft.AspNetCore.Components;
using WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;
using WebAppEstimate.Areas.SiteBram.Services;
//using WebAppEstimate.Areas.SiteBram.Entity;

namespace WebAppEstimate.Areas.SiteBram.Components.Pages;

public partial class WinchAnchorPage
{
    [Inject] private IWinchAnchorService WinchAnchorService { get; set; } = null!;

    protected List<WinchAnchorSeries> Series { get; set; } = new();

    protected Guid? SelectedSeriesId { get; set; }

    protected int ValueKg { get; set; }

    protected int ValueMm { get; set; }

    protected int ValueKgMm { get; set; }

    protected WinchAnchorDesign? Result { get; set; }

    protected string Message { get; set; } = string.Empty;

    protected bool CanSave => Result != null;


    protected override async Task OnInitializedAsync()
    {
        Series = await WinchAnchorService.GetSeriesAsync();
    }


    protected async Task CalculateAsync()
    {
        Message = string.Empty;
        Result = null;

        if (SelectedSeriesId == null)
        {
            Message = "Выберите серию лебёдки.";
            return;
        }

        Result = await WinchAnchorService.FindDesignAsync(
            SelectedSeriesId.Value,
            ValueKg,
            ValueMm,
            ValueKgMm);

        if (Result == null)
            Message =
                "Подходящая конфигурация якорной лебёдки не найдена.";
    }


    protected Task SaveAsync()
    {
        if (Result == null)
            return Task.CompletedTask;

        // Позже здесь сделаем сохранение результата
        // в History, как у центробежных насосов.

        Message = "Результат подготовлен к сохранению.";

        return Task.CompletedTask;
    }


    protected void Clear()
    {
        SelectedSeriesId = null;

        ValueKg = 0;
        ValueMm = 0;
        ValueKgMm = 0;

        Result = null;
        Message = string.Empty;
    }
}