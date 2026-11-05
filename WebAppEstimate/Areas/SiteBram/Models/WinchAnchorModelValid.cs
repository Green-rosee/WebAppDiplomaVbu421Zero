using FluentValidation;

namespace WebAppEstimate.Areas.SiteBram.Models;


public class WinchAnchorModelValid : AbstractValidator<WinchAnchorModel>
{
    public WinchAnchorModelValid()
    {
        RuleFor(x => x.SeriesId)
            .NotNull()
            .WithMessage("Выберите серию лебёдки.");

        RuleFor(x => x.ValueKg)
            .GreaterThan(0)
            .WithMessage("Масса должна быть больше 0.");

        RuleFor(x => x.ValueMm)
            .GreaterThan(0)
            .WithMessage("Диаметр вала должен быть больше 0.");

        RuleFor(x => x.ValueKgMm)
            .GreaterThan(0)
            .WithMessage("Калибр цепи должен быть больше 0.");

        RuleFor(x => x.HourCost)
            .GreaterThan(0)
            .WithMessage("Стоимость часа должна быть больше 0.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Количество должно быть больше 0.");
    }
}






/*public class WinchAnchorModelValid : AbstractValidator<WinchAnchorModel>
{
    public WinchAnchorModelValid()
    {
        RuleFor(x => x.SeriesId)
            .NotNull()
            .WithMessage("Выберите серию лебёдки.");

        RuleFor(x => x.ValueKg)
            .GreaterThan(0)
            .WithMessage("Масса должна быть больше 0.");

        RuleFor(x => x.ValueMm)
            .GreaterThan(0)
            .WithMessage("Диаметр вала должен быть больше 0.");

        RuleFor(x => x.ValueKgMm)
            .GreaterThan(0)
            .WithMessage("Параметр цепи должен быть больше 0.");

        RuleFor(x => x.CostHour)
            .GreaterThan(0)
            .WithMessage("Стоимость часа должна быть больше 0.");
    }
}*/
    /*public WinchAnchorModelValid()
    {
        RuleFor(x => x.SeriesId)
            .NotNull()
            .WithMessage("Выберите серию лебёдки.");

        RuleFor(x => x.ValueKg)
            .GreaterThan(0)
            .WithMessage("Масса должна быть больше 0.");

        RuleFor(x => x.ValueMm)
            .GreaterThan(0)
            .WithMessage("Диаметр вала должен быть больше 0.");

        RuleFor(x => x.ValueKgMm)
            .GreaterThan(0)
            .WithMessage("Параметр цепи должен быть больше 0.");
    }*/
