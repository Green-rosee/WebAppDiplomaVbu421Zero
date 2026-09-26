using Microsoft.AspNetCore.Components;
using BlazorDatasheet.Core.Data;
using BlazorDatasheet.Core.Formats;
using WebAppEstimate.Areas.SiteBram.Models;
using WebAppEstimate.Areas.SiteBram.Services;

namespace WebAppEstimate.Areas.SiteBram.Components.Pages;

public partial class WinchAnchorSheetPage
{
    [Inject]
    private IWinchAnchorExcelExportService ExcelExportService
    { get; set; } = null!;

    protected string SaveMessage { get; set; } = string.Empty;
    //--------------
    [Inject]
    private IWinchAnchorHistoryService HistoryService { get; set; } = null!;
    
    private int _calculationCount;

    protected Sheet Sheet { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var calculations =
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
        }
    }
    
    protected async Task SaveExcelAsync()
    {
        var rows = new List<List<object?>>();

        // Заголовок + реальные строки расчётов
        for (var row = 0; row <= _calculationCount; row++)
        {
            var values = new List<object?>();

            for (var col = 0; col < 8; col++)
            {
                var cell = Sheet.Cells[row, col];

                values.Add(
                    cell.Value?.ToString());
            }

            rows.Add(values);
        }

        var path =
            await ExcelExportService.SaveContractAsync(rows);

        SaveMessage =
            $"Excel-файл сохранён: {Path.GetFileName(path)}";
    }
}