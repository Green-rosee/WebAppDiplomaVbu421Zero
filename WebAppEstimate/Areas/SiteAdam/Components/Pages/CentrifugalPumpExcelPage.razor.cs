using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Spreadsheet;
using WebAppEstimate.Areas.SiteAdam.Services;

namespace WebAppEstimate.Areas.SiteAdam.Components.Pages;

public partial class CentrifugalPumpExcelPage
{
    protected string EditMessage =
        "Редактирование ещё не выполнялось.";

    protected string SaveMessage = string.Empty;

    protected SfSpreadsheet Spreadsheet = null!;

    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;

    [Inject] private ICentrifugalPumpHistoryService HistoryService { get; set; } = null!;

    [Inject] private IWebHostEnvironment WebHostEnvironment { get; set; } = null!;

    /// <summary>
    ///     Сохраняет текущее состояние интерактивной Excel-таблицы
    ///     центробежных насосов в файл на сервере приложения.
    /// </summary>
    protected async Task SaveExcelToFolder()
    {
        var folderPath = Path.Combine(
            WebHostEnvironment.ContentRootPath,
            "ExcelFilesPageSave",
            "CentrPumps");

        Directory.CreateDirectory(folderPath);

        var fileName =
            $"CentrifugalPumps_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx";

        var filePath =
            Path.Combine(
                folderPath,
                fileName);

        using var stream =
            await Spreadsheet.SaveAsStreamAsync();

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

    /// <summary>
    ///     Загружает актуальную историю расчётов из базы данных
    ///     и отображает её в интерактивной Excel-таблице.
    ///     Перед заполнением очищает ранее отображавшиеся строки,
    ///     чтобы Spreadsheet соответствовал текущему состоянию базы.
    /// </summary>
    protected async Task LoadData()
    {
        var histories =
            await HistoryService.GetAllAsync();

        var clearUpdates =
            new List<CellUpdateItem>();

        for (var clearRow = 2;
             clearRow <= 200;
             clearRow++)
        for (var column = 'A';
             column <= 'I';
             column++)
            clearUpdates.Add(
                new CellUpdateItem
                {
                    CellAddress =
                        $"Sheet1!{column}{clearRow}",

                    Value = ""
                });

        await Spreadsheet
            .UpdateCellsAsync(clearUpdates);

        var updates =
            new List<CellUpdateItem>
            {
                new()
                {
                    CellAddress = "Sheet1!A1",
                    Value = "Наименование"
                },
                new()
                {
                    CellAddress = "Sheet1!B1",
                    Value = "Количество"
                },
                new()
                {
                    CellAddress = "Sheet1!C1",
                    Value = "Стоимость часа"
                },
                new()
                {
                    CellAddress = "Sheet1!D1",
                    Value = "Крылатка, мм"
                },
                new()
                {
                    CellAddress = "Sheet1!E1",
                    Value = "Производительность, м³/ч"
                },
                new()
                {
                    CellAddress = "Sheet1!F1",
                    Value = "Масса, кг"
                },
                new()
                {
                    CellAddress = "Sheet1!G1",
                    Value = "Трудоёмкость"
                },
                new()
                {
                    CellAddress = "Sheet1!H1",
                    Value = "Общая стоимость"
                },
                new()
                {
                    CellAddress = "Sheet1!I1",
                    Value = "Дата"
                }
            };

        var row = 2;

        foreach (var item in histories)
        {
            updates.Add(
                new CellUpdateItem
                {
                    CellAddress =
                        $"Sheet1!A{row}",

                    Value = item.Name
                });

            updates.Add(
                new CellUpdateItem
                {
                    CellAddress =
                        $"Sheet1!B{row}",

                    Value = item.Number
                });

            updates.Add(
                new CellUpdateItem
                {
                    CellAddress =
                        $"Sheet1!C{row}",

                    Value = item.CostHour
                });

            updates.Add(
                new CellUpdateItem
                {
                    CellAddress =
                        $"Sheet1!D{row}",

                    Value = item.Impeller
                });

            updates.Add(
                new CellUpdateItem
                {
                    CellAddress =
                        $"Sheet1!E{row}",

                    Value = item.Volume
                });

            updates.Add(
                new CellUpdateItem
                {
                    CellAddress =
                        $"Sheet1!F{row}",

                    Value = item.Weight
                });

            updates.Add(
                new CellUpdateItem
                {
                    CellAddress =
                        $"Sheet1!G{row}",

                    Value = item.TotalSumHour
                });

            updates.Add(
                new CellUpdateItem
                {
                    CellAddress =
                        $"Sheet1!H{row}",

                    Value = item.TotalSumCostHour
                });

            updates.Add(
                new CellUpdateItem
                {
                    CellAddress =
                        $"Sheet1!I{row}",

                    Value = item.CreatedAt
                        .ToLocalTime()
                        .ToString("dd.MM.yyyy")
                });

            row++;
        }

        await Spreadsheet
            .UpdateCellsAsync(updates);
    }

    /// <summary>
    ///     Устанавливает ширину столбцов Excel-таблицы
    ///     для более удобного отображения расчётных данных.
    /// </summary>
    protected async Task SetColumnWidths()
    {
        await Spreadsheet
            .SetColumnWidthAsync(
                180,
                0,
                0);

        await Spreadsheet
            .SetColumnWidthAsync(
                90,
                1,
                0);

        await Spreadsheet
            .SetColumnWidthAsync(
                120,
                2,
                0);

        await Spreadsheet
            .SetColumnWidthAsync(
                110,
                3,
                0);

        await Spreadsheet
            .SetColumnWidthAsync(
                180,
                4,
                0);

        await Spreadsheet
            .SetColumnWidthAsync(
                100,
                5,
                0);

        await Spreadsheet
            .SetColumnWidthAsync(
                120,
                6,
                0);

        await Spreadsheet
            .SetColumnWidthAsync(
                150,
                7,
                0);

        await Spreadsheet
            .SetColumnWidthAsync(
                120,
                8,
                0);
    }

    /// <summary>
    ///     Обрабатывает начало редактирования ячейки
    ///     и сохраняет информацию о текущей редактируемой ячейке.
    /// </summary>
    /// <param name="args">
    ///     Данные события редактирования ячейки.
    /// </param>
    protected void OnCellEditing(
        CellEditingEventArgs args)
    {
        EditMessage =
            $"Редактируется ячейка: {args.Address}";
    }

    /// <summary>
    ///     Обрабатывает завершение редактирования ячейки
    ///     и сохраняет информацию о последней изменённой ячейке.
    /// </summary>
    /// <param name="args">
    ///     Данные события сохранения ячейки.
    /// </param>
    protected void OnCellSaved(
        CellSavedEventArgs args)
    {
        EditMessage =
            $"Сохранено: {args.Address}";
    }

    /// <summary>
    ///     Формирует контракт на основе текущего состояния Spreadsheet
    ///     и сохраняет полученный Excel-файл в папке сервера.
    /// </summary>
    protected async Task SaveContractExcel()
    {
        using var workbook =
            await BuildContractFromSpreadsheetAsync();

        var folderPath =
            Path.Combine(
                WebHostEnvironment.ContentRootPath,
                "ExcelFilesPageSave",
                "ContractsCentrPumps");

        Directory.CreateDirectory(
            folderPath);

        var fileName =
            $"Contract_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx";

        var filePath =
            Path.Combine(
                folderPath,
                fileName);

        workbook.SaveAs(
            filePath);

        SaveMessage =
            $"Контракт сохранён на сервере: {fileName}";
    }

    /// <summary>
    ///     Формирует контракт на основе текущего состояния Spreadsheet
    ///     и передаёт полученный Excel-файл пользователю для скачивания.
    /// </summary>
    protected async Task DownloadContractAsync()
    {
        using var workbook =
            await BuildContractFromSpreadsheetAsync();

        using var stream =
            new MemoryStream();

        workbook.SaveAs(
            stream);

        var fileBytes =
            stream.ToArray();

        var base64 =
            Convert.ToBase64String(
                fileBytes);

        var fileName =
            $"Contract_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx";

        await JsRuntime.InvokeVoidAsync(
            "downloadFileFromBase64",
            fileName,
            base64);

        SaveMessage =
            $"Контракт подготовлен к скачиванию: {fileName}";
    }

    /// <summary>
    ///     Выполняет первоначальную настройку страницы
    ///     после первого отображения компонента:
    ///     загружает актуальные данные из базы
    ///     и устанавливает ширину столбцов.
    /// </summary>
    /// <param name="firstRender">
    ///     Указывает, выполняется ли первый рендер компонента.
    /// </param>
    protected override async Task OnAfterRenderAsync(
        bool firstRender)
    {
        if (firstRender)
        {
            await LoadData();

            await SetColumnWidths();
        }
    }

    /// <summary>
    ///     Формирует Excel-файл из текущего состояния Spreadsheet
    ///     и передаёт его пользователю для скачивания
    ///     на локальный компьютер.
    /// </summary>
    protected async Task DownloadExcelAsync()
    {
        using var stream =
            await Spreadsheet.SaveAsStreamAsync();

        stream.Position = 0;

        using var memoryStream =
            new MemoryStream();

        await stream.CopyToAsync(
            memoryStream);

        var fileBytes =
            memoryStream.ToArray();

        var base64 =
            Convert.ToBase64String(
                fileBytes);

        var fileName =
            $"CentrifugalPumps_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx";

        await JsRuntime.InvokeVoidAsync(
            "downloadFileFromBase64",
            fileName,
            base64);

        SaveMessage =
            $"Excel подготовлен к скачиванию: {fileName}";
    }

    /// <summary>
    ///     Создаёт Excel-контракт на основе текущего содержимого Spreadsheet.
    ///     В контракт включаются данные, загруженные из базы,
    ///     а также дополнительные строки и изменения,
    ///     внесённые пользователем непосредственно в таблице.
    /// </summary>
    /// <returns>
    ///     Сформированная Excel-книга контракта.
    /// </returns>
    private async Task<XLWorkbook>
        BuildContractFromSpreadsheetAsync()
    {
        using var spreadsheetStream =
            await Spreadsheet.SaveAsStreamAsync();

        spreadsheetStream.Position = 0;

        using var sourceWorkbook =
            new XLWorkbook(
                spreadsheetStream);

        var sourceSheet =
            sourceWorkbook.Worksheet(
                "Sheet1");

        var contractWorkbook =
            new XLWorkbook();

        var contractSheet =
            contractWorkbook.Worksheets.Add(
                "Контракт");

        contractSheet.Cell("A1").Value =
            "КОНТРАКТ";

        contractSheet.Cell("A3").Value =
            "Заказчик:";

        contractSheet.Cell("B3").Value =
            "ООО Заказчик";

        contractSheet.Cell("A4").Value =
            "Дата:";

        contractSheet.Cell("B4").Value =
            DateTime.Now;

        contractSheet.Cell("A6").Value =
            "Наименование";

        contractSheet.Cell("B6").Value =
            "Количество";

        contractSheet.Cell("C6").Value =
            "Стоимость часа";

        contractSheet.Cell("D6").Value =
            "Общая стоимость";

        var contractRow = 7;

        var lastRow =
            sourceSheet
                .LastRowUsed()?
                .RowNumber()
            ?? 1;

        for (var sourceRow = 2;
             sourceRow <= lastRow;
             sourceRow++)
        {
            var name =
                sourceSheet
                    .Cell(
                        sourceRow,
                        1)
                    .GetString()
                    .Trim();

            if (string.IsNullOrWhiteSpace(
                    name))
                continue;

            contractSheet
                .Cell(
                    contractRow,
                    1)
                .Value = name;

            contractSheet
                    .Cell(
                        contractRow,
                        2)
                    .Value =
                sourceSheet
                    .Cell(
                        sourceRow,
                        2)
                    .Value;

            contractSheet
                    .Cell(
                        contractRow,
                        3)
                    .Value =
                sourceSheet
                    .Cell(
                        sourceRow,
                        3)
                    .Value;

            contractSheet
                    .Cell(
                        contractRow,
                        4)
                    .Value =
                sourceSheet
                    .Cell(
                        sourceRow,
                        8)
                    .Value;

            contractRow++;
        }

        contractSheet
                .Cell(
                    contractRow + 1,
                    3)
                .Value =
            "ИТОГО:";

        contractSheet
                .Cell(
                    contractRow + 1,
                    4)
                .FormulaA1 =
            $"SUM(D7:D{contractRow - 1})";

        contractSheet
            .Columns()
            .AdjustToContents();

        return contractWorkbook;
    }
}

//------------------------------------------
