using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Spreadsheet;
using WebAppEstimate.Areas.SiteAdam.Services;

namespace WebAppEstimate.Areas.SiteAdam.Components.Pages;

public partial class CentrifugalPumpExcelPage
{
    [Inject]
    private ICentrifugalPumpHistoryService HistoryService { get; set; } = null!;

    protected SfSpreadsheet Spreadsheet = null!;

    protected async Task LoadData()
    {
        
        var histories = await HistoryService.GetAllAsync();

        await Spreadsheet.UpdateCellAsync("Sheet1!A1", "Наименование");
        await Spreadsheet.UpdateCellAsync("Sheet1!B1", "Количество");
        await Spreadsheet.UpdateCellAsync("Sheet1!C1", "Стоимость часа");
        await Spreadsheet.UpdateCellAsync("Sheet1!D1", "Крылатка, мм");
        await Spreadsheet.UpdateCellAsync("Sheet1!E1", "Производительность, м³/ч");
        await Spreadsheet.UpdateCellAsync("Sheet1!F1", "Масса, кг");
        await Spreadsheet.UpdateCellAsync("Sheet1!G1", "Трудоёмкость");
        await Spreadsheet.UpdateCellAsync("Sheet1!H1", "Общая стоимость");
        await Spreadsheet.UpdateCellAsync("Sheet1!I1", "Дата");

        var row = 2;

        foreach (var item in histories)
        {
            await Spreadsheet.UpdateCellAsync($"Sheet1!A{row}", item.Name);
            await Spreadsheet.UpdateCellAsync($"Sheet1!B{row}", item.Number);
            await Spreadsheet.UpdateCellAsync($"Sheet1!C{row}", item.CostHour);
            await Spreadsheet.UpdateCellAsync($"Sheet1!D{row}", item.Impeller);
            await Spreadsheet.UpdateCellAsync($"Sheet1!E{row}", item.Volume);
            await Spreadsheet.UpdateCellAsync($"Sheet1!F{row}", item.Weight);
            await Spreadsheet.UpdateCellAsync($"Sheet1!G{row}", item.TotalSumHour);
            await Spreadsheet.UpdateCellAsync($"Sheet1!H{row}", item.TotalSumCostHour);
            await Spreadsheet.UpdateCellAsync(
                $"Sheet1!I{row}",
                item.CreatedAt.ToLocalTime());

            row++;
        }

        var lastDataRow = row - 1;

        await Spreadsheet.CellFormatAsync(
            new CellFormat
            {
                FontWeight = FontWeight.Bold,
                TextAlign = TextAlign.Center
            },
            "Sheet1!A1:I1");

        await Spreadsheet.NumberFormatAsync(
            "#,##0.00",
            $"Sheet1!C2:C{lastDataRow}");

        await Spreadsheet.NumberFormatAsync(
            "#,##0.00",
            $"Sheet1!G2:G{lastDataRow}");

        await Spreadsheet.NumberFormatAsync(
            "#,##0.00",
            $"Sheet1!H2:H{lastDataRow}");

        await Spreadsheet.NumberFormatAsync(
            "dd.mm.yyyy",
            $"Sheet1!I2:I{lastDataRow}");

        var totalRow = row + 1;

        await Spreadsheet.UpdateCellAsync(
            $"Sheet1!F{totalRow}",
            "ИТОГО:");

        await Spreadsheet.UpdateCellAsync(
            $"Sheet1!G{totalRow}",
            $"=SUM(G2:G{lastDataRow})");

        await Spreadsheet.UpdateCellAsync(
            $"Sheet1!H{totalRow}",
            $"=SUM(H2:H{lastDataRow})");

        await Spreadsheet.CellFormatAsync(
            new CellFormat
            {
                FontWeight = FontWeight.Bold
            },
            $"Sheet1!F{totalRow}:H{totalRow}");

        await Spreadsheet.NumberFormatAsync(
            "#,##0.00",
            $"Sheet1!G{totalRow}:H{totalRow}");

        
    }
    
    protected async Task SetColumnWidths()
    {
        //await Task.Delay(300);

        await Spreadsheet.SetColumnWidthAsync(180, 0, 0);
        await Spreadsheet.SetColumnWidthAsync(90, 1, 0);
        await Spreadsheet.SetColumnWidthAsync(120, 2, 0);
        await Spreadsheet.SetColumnWidthAsync(110, 3, 0);
        await Spreadsheet.SetColumnWidthAsync(180, 4, 0);
        await Spreadsheet.SetColumnWidthAsync(100, 5, 0);
        await Spreadsheet.SetColumnWidthAsync(120, 6, 0);
        await Spreadsheet.SetColumnWidthAsync(150, 7, 0);
        await Spreadsheet.SetColumnWidthAsync(120, 8, 0);
    }
    
    /*protected async Task PrepareTable()
    {
        await SetColumnWidths();
        await Task.Delay(800);
        await LoadData();
    }*/
}