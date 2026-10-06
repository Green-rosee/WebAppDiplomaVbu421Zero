using FluentValidation;
using Microsoft.AspNetCore.Components;
using WebAppEstimate.Areas.SiteCerber.Models;
using WebAppEstimate.Areas.SiteCerber.Services;

namespace WebAppEstimate.Areas.SiteCerber.Components;

public partial class FlangePage
{
    [Inject] protected IOpenScadService OpenScad { get; set; } = null!;

    [Inject] protected IGigaChatService GigaChat { get; set; } = null!;

    [Inject] protected IValidator<FlangeModel> Validator { get; set; } = null!;

    [Inject] protected IFlangeDrawingService DrawingService { get; set; } = null!;


    protected string? DrawingUrl { get; set; }
    protected string AiPrompt { get; set; } = string.Empty;

    protected string? ResultPath { get; set; }
    protected string? Message { get; set; }
    protected string? PreviewUrl { get; set; }


    protected FlangeModel Model { get; set; } = new()
    {
        OuterDiameter = 100,
        InnerDiameter = 50,
        Thickness = 20,
        HoleCount = 6,
        HoleDiameter = 10,
        BoltCircleDiameter = 80
    };


    protected async Task GenerateAsync()
    {
        try
        {
            var validationResult =
                await Validator.ValidateAsync(Model);

            if (!validationResult.IsValid)
            {
                Message =
                    string.Join(
                        Environment.NewLine,
                        validationResult.Errors.Select(x => x.ErrorMessage));

                return;
            }

            ResultPath =
                await OpenScad.GenerateFlangeAsync(Model);

            var fileName =
                Path.GetFileNameWithoutExtension(ResultPath);

            PreviewUrl =
                $"/generated/sitecerber/{fileName}.png";

            DrawingUrl =
                await DrawingService.GenerateSvgAsync(Model);
            Console.WriteLine($"DrawingUrl = {DrawingUrl}");

            Message = "Модель и чертёж успешно созданы.";
        }
        catch (Exception ex)
        {
            Message = ex.Message;
        }
    }

    protected async Task FillFromAiAsync()
    {
        try
        {
            Message = "Отправляю запрос в GigaChat...";

            var flange =
                await GigaChat.ParseFlangeAsync(AiPrompt);

            if (flange != null)
            {
                Model = flange;
                Message = "Параметры заполнены с помощью GigaChat.";
            }
            else
            {
                Message = "GigaChat вернул пустой результат.";
            }
        }
        catch (Exception ex)
        {
            Message = $"Ошибка GigaChat: {ex.Message}";
        }
    }
}