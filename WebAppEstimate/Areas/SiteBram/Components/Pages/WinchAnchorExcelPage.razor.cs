using Microsoft.AspNetCore.Components;
using BlazorDatasheet.Core.Data;
using BlazorDatasheet.Core.Formats;
using WebAppEstimate.Areas.SiteBram.Models;
using WebAppEstimate.Areas.SiteBram.Services;

namespace WebAppEstimate.Areas.SiteBram.Components.Pages;

public partial class WinchAnchorExcelPage
{
    [Inject]
    private IWinchAnchorExcelExportService ExcelExportService
    { get; set; } = null!;

    [Inject]
    private IWinchAnchorHistoryService HistoryService { get; set; } = null!;
    
    
    protected Sheet Sheet { get; set; } = null!;
    protected bool IsLoading { get; set; }

    protected string Message { get; set; } = string.Empty;

    private int _calculationCount;
    
    //----------------

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
        
        /*var calculations =
            await HistoryService.GetAllAsync();
        
        _calculationCount = calculations.Count;

        Sheet = new Sheet(
            Math.Max(calculations.Count + 5, 20),
            10);

        Sheet.Range("A1").Value = "Название";
        Sheet.Range("B1").Value = "Серия";
        Sheet.Range("C1").Value = "Масса";
        Sheet.Range("D1").Value = "Диаметр вала";
        Sheet.Range("E1").Value = "Калибр цепи";
        Sheet.Range("F1").Value = "Трудоёмкость";
        Sheet.Range("G1").Value = "Стоимость часа";
        Sheet.Range("H1").Value = "Итоговая стоимость";

        for (var i = 0; i < calculations.Count; i++)
        {
            var item = calculations[i];

            var row = i + 2;

            Sheet.Range($"A{row}").Value = item.WinchName;
            Sheet.Range($"B{row}").Value = item.SeriesName;

            Sheet.Range($"C{row}").Value = item.SelectedValueKg;
            Sheet.Range($"D{row}").Value = item.SelectedValueMm;
            Sheet.Range($"E{row}").Value = item.SelectedValueKgMm;

            Sheet.Range($"F{row}").Value = item.Hour;
            Sheet.Range($"G{row}").Value = item.HourCost;
            Sheet.Range($"H{row}").Value = item.TotalCost;
        }*/
    }
    
    private async Task LoadDataAsync()
    {
        IsLoading = true;
        Message = string.Empty;

        try
        {
            var calculations =
                await HistoryService.GetAllAsync();

            _calculationCount = calculations.Count;

            var newSheet = new Sheet(
                Math.Max(calculations.Count + 5, 20),
                10);

            newSheet.Range("A1").Value = "Название";
            newSheet.Range("B1").Value = "Серия";
            newSheet.Range("C1").Value = "Количество";
            newSheet.Range("D1").Value = "Масса";
            newSheet.Range("E1").Value = "Диаметр вала";
            newSheet.Range("F1").Value = "Калибр цепи";
            newSheet.Range("G1").Value = "Трудоёмкость";
            newSheet.Range("H1").Value = "Стоимость часа";
            newSheet.Range("I1").Value = "Итоговая стоимость";

            for (var i = 0; i < calculations.Count; i++)
            {
                var item = calculations[i];

                var row = i + 2;

                newSheet.Range($"A{row}").Value =
                    item.WinchName;

                newSheet.Range($"B{row}").Value =
                    item.SeriesName;

                newSheet.Range($"C{row}").Value =
                    item.Quantity;

                newSheet.Range($"D{row}").Value =
                    item.SelectedValueKg;

                newSheet.Range($"E{row}").Value =
                    item.SelectedValueMm;

                newSheet.Range($"F{row}").Value =
                    item.SelectedValueKgMm;

                newSheet.Range($"G{row}").Value =
                    item.Hour;

                newSheet.Range($"H{row}").Value =
                    item.HourCost;

                newSheet.Range($"I{row}").Value =
                    item.TotalCost;
            }

            Sheet = newSheet;

            Message =
                $"Загружено расчётов: {calculations.Count}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    
    protected async Task RefreshAsync()
    {
        await LoadDataAsync();
    }
    
    protected async Task SaveExcelAsync()
    {
        var rows = new List<List<object?>>();

        // Заголовок + сохранённые расчёты
        for (var row = 0; row <= _calculationCount; row++)
        {
            var values = new List<object?>();

            for (var col = 0; col < 9; col++)
            {
                var cell = Sheet.Cells[row, col];

                values.Add(
                    cell.Value?.ToString());
            }

            rows.Add(values);
        }

        var path =
            await ExcelExportService.SaveContractAsync(rows);

        Message =
            $"Excel-файл сохранён: {Path.GetFileName(path)}";
    }
}