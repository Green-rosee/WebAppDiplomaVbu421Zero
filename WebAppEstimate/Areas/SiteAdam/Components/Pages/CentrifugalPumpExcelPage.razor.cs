using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Spreadsheet;
using WebAppEstimate.Areas.SiteAdam.Services;

using Microsoft.AspNetCore.Hosting;

namespace WebAppEstimate.Areas.SiteAdam.Components.Pages;

public partial class CentrifugalPumpExcelPage
{
    [Inject]
    private ICentrifugalPumpHistoryService HistoryService { get; set; } = null!;
    
    [Inject]
    private IWebHostEnvironment WebHostEnvironment { get; set; } = null!;

    protected SfSpreadsheet Spreadsheet = null!;

    protected string EditMessage =
        "Редактирование ещё не выполнялось.";

    protected string SaveMessage = string.Empty;
    
    protected async Task SaveExcelToFolder()
    {
        // Папка:
        // WebAppEstimate/ExcelFilesPageSave/CentrPumps
        var folderPath = Path.Combine(
            WebHostEnvironment.ContentRootPath,
            "ExcelFilesPageSave",
            "CentrPumps");

        // Если папки вдруг нет — создадим
        Directory.CreateDirectory(folderPath);

        var fileName =
            $"CentrifugalPumps_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx";

        var filePath = Path.Combine(
            folderPath,
            fileName);

        // Получаем именно текущее состояние Spreadsheet
        using var stream =
            await Spreadsheet.SaveAsStreamAsync();

        // Записываем его на диск
        await using var fileStream =
            new FileStream(
                filePath,
                FileMode.Create,
                FileAccess.Write);

        stream.Position = 0;

        await stream.CopyToAsync(fileStream);

        SaveMessage =
            $"Excel сохранён: {fileName}";
    }
    
    protected async Task LoadData()
{
    var histories = await HistoryService.GetAllAsync();

    var updates = new List<CellUpdateItem>
    {
        new() { CellAddress = "Sheet1!A1", Value = "Наименование" },
        new() { CellAddress = "Sheet1!B1", Value = "Количество" },
        new() { CellAddress = "Sheet1!C1", Value = "Стоимость часа" },
        new() { CellAddress = "Sheet1!D1", Value = "Крылатка, мм" },
        new() { CellAddress = "Sheet1!E1", Value = "Производительность, м³/ч" },
        new() { CellAddress = "Sheet1!F1", Value = "Масса, кг" },
        new() { CellAddress = "Sheet1!G1", Value = "Трудоёмкость" },
        new() { CellAddress = "Sheet1!H1", Value = "Общая стоимость" },
        new() { CellAddress = "Sheet1!I1", Value = "Дата" }
    };

    var row = 2;

    foreach (var item in histories)
    {
        updates.Add(new CellUpdateItem
        {
            CellAddress = $"Sheet1!A{row}",
            Value = item.Name
        });

        updates.Add(new CellUpdateItem
        {
            CellAddress = $"Sheet1!B{row}",
            Value = item.Number
        });

        updates.Add(new CellUpdateItem
        {
            CellAddress = $"Sheet1!C{row}",
            Value = item.CostHour
        });

        updates.Add(new CellUpdateItem
        {
            CellAddress = $"Sheet1!D{row}",
            Value = item.Impeller
        });

        updates.Add(new CellUpdateItem
        {
            CellAddress = $"Sheet1!E{row}",
            Value = item.Volume
        });

        updates.Add(new CellUpdateItem
        {
            CellAddress = $"Sheet1!F{row}",
            Value = item.Weight
        });

        updates.Add(new CellUpdateItem
        {
            CellAddress = $"Sheet1!G{row}",
            Value = item.TotalSumHour
        });

        updates.Add(new CellUpdateItem
        {
            CellAddress = $"Sheet1!H{row}",
            Value = item.TotalSumCostHour
        });

        updates.Add(new CellUpdateItem
        {
            CellAddress = $"Sheet1!I{row}",
            Value = item.CreatedAt
                .ToLocalTime()
                .ToString("dd.MM.yyyy")
        });

        row++;
    }

    // Один JS/Blazor-вызов вместо десятков
    await Spreadsheet.UpdateCellsAsync(updates);
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
    
   //---
   protected void OnCellEditing(CellEditingEventArgs args)
   {
       EditMessage = $"Редактируется ячейка: {args.Address}";
   }

   protected void OnCellSaved(CellSavedEventArgs args)
   {
       
       EditMessage = $"Сохранено: {args.Address}";
   }
   
   //---
   protected async Task SaveContractExcel()
   {
       var histories = await HistoryService.GetAllAsync();

       using var workbook = new XLWorkbook();

       var sheet = workbook.Worksheets.Add("Контракт");

       sheet.Cell("A1").Value = "КОНТРАКТ";
       sheet.Cell("A3").Value = "Заказчик:";
       sheet.Cell("B3").Value = "ООО Заказчик";

       sheet.Cell("A4").Value = "Дата:";
       sheet.Cell("B4").Value = DateTime.Now;

       sheet.Cell("A6").Value = "Наименование";
       sheet.Cell("B6").Value = "Количество";
       sheet.Cell("C6").Value = "Стоимость часа";
       sheet.Cell("D6").Value = "Общая стоимость";

       var row = 7;

       foreach (var item in histories)
       {
           sheet.Cell(row, 1).Value = item.Name;
           sheet.Cell(row, 2).Value = item.Number;
           sheet.Cell(row, 3).Value = item.CostHour;
           sheet.Cell(row, 4).Value = item.TotalSumCostHour;

           row++;
       }

       sheet.Cell(row + 1, 3).Value = "ИТОГО:";
       sheet.Cell(row + 1, 4).FormulaA1 =
           $"SUM(D7:D{row - 1})";

       sheet.Columns().AdjustToContents();

       var folderPath = Path.Combine(
           WebHostEnvironment.ContentRootPath,
           "ExcelFilesPageSave",
           "ContractsCentrPumps");

       Directory.CreateDirectory(folderPath);

       var fileName =
           $"Contract_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx";

       var filePath = Path.Combine(folderPath, fileName);

       workbook.SaveAs(filePath);
   }
}

/*protected async Task LoadData()
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
         await Spreadsheet.UpdateCellAsync("Sheet1!I1", "Коментарии");
 
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
 
         
     }*/