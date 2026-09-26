using Microsoft.AspNetCore.Components;
using WebAppEstimate.Areas.SiteBram.Data.Entity.WinchAnchors;
using WebAppEstimate.Areas.SiteBram.Models;
using WebAppEstimate.Areas.SiteBram.Services;
//using WebAppEstimate.Areas.SiteBram.Entity;

namespace WebAppEstimate.Areas.SiteBram.Components.Pages;

public partial class WinchAnchorPage
{
    [Inject]
    private IWinchAnchorHistoryService HistoryService { get; set; } = null!;

    private Guid? _savedCalculationId;
    //------------
    
    /*[Inject]
    private WinchAnchorExcelState ExcelState { get; set; } = null!;*/

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;
    
    [Inject] private IWinchAnchorService WinchAnchorService { get; set; } = null!;

    protected List<WinchAnchorSeries> Series { get; set; } = new();

    protected Guid? SelectedSeriesId { get; set; }

    protected int ValueKg { get; set; }

    protected int ValueMm { get; set; }

    protected int ValueKgMm { get; set; }

    protected WinchAnchorDesign? Result { get; set; }

    protected string Message { get; set; } = string.Empty;

    //protected bool CanSave => Result != null;
    
    protected decimal HourCost { get; set; }

    protected decimal TotalCost =>
        Result is null
            ? 0
            : (decimal)Result.Hour * HourCost;
    
    protected bool CanSave =>
        Result != null && HourCost > 0;
    
    protected string WinchName { get; set; } = string.Empty;
    
    protected bool CanShowExcel =>
        _savedCalculationId != null;

    //--------------------------------------------------
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


    protected async Task SaveAsync()
    {
        if (Result == null || SelectedSeriesId == null)
            return;

        var seriesName = Series
            .FirstOrDefault(x => x.Id == SelectedSeriesId.Value)?
            .Name ?? string.Empty;

        var history = new WinchAnchorCalculationHistory
        {
            WinchName = WinchName,

            WinchAnchorSeriesId = SelectedSeriesId.Value,
            SeriesName = seriesName,

            InputValueKg = ValueKg,
            InputValueMm = ValueMm,
            InputValueKgMm = ValueKgMm,

            SelectedValueKg = Result.ValueKg,
            SelectedValueMm = Result.ValueMm,
            SelectedValueKgMm = Result.ValueKgMm,

            Hour = Result.Hour,
            HourCost = HourCost,
            TotalCost = TotalCost
        };

        _savedCalculationId =
            await HistoryService.SaveAsync(history);

        Message = "Расчёт сохранён.";
    }
    
    protected  Task ShowExcel()
    {
        NavigationManager.NavigateTo(
            "/SiteBram/WinchAnchor/Sheet");

        return Task.CompletedTask;
        
    }


    protected void Clear()
    {
        SelectedSeriesId = null;

        ValueKg = 0;
        ValueMm = 0;
        ValueKgMm = 0;  
        HourCost = 0;
        
        Result = null;
        Message = string.Empty;
        WinchName = string.Empty;
        
        _savedCalculationId = null;
       
    }
}