using FluentValidation;

namespace WebAppEstimate.Areas.SiteAdam.Models;

public class CentrifugalPumpModelValid : AbstractValidator<CentrifugalPumpModel>
{
    public CentrifugalPumpModelValid()
    {
        //валид. стоимтсь часа
        RuleFor(x => x.CostHour)
            .GreaterThan(0).WithMessage("Введите стоимость часа работ.")
            .LessThanOrEqualTo(100000).WithMessage("Указан слишком большая цена за час");


        // Валидация названия насоса
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Введите название или марку насоса")
            .MaximumLength(100).WithMessage("Название не должно превышать 100 символов");

        // Валидация выбора серии
        RuleFor(x => x.SelectedSeriesId)
            .GreaterThan(0).WithMessage("Выберите серию насоса из реестра");

        // Валидация диаметра крыльчатки
        RuleFor(x => x.Impeller)
            .GreaterThan(0).WithMessage("Диаметр крыльчатки должен быть больше нуля")
            .LessThanOrEqualTo(500).WithMessage("Указан слишком большой диаметр крыльчатки");

        // Валидация веса
        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("Вес должен быть больше нуля")
            .LessThanOrEqualTo(1000).WithMessage("Указан слишком большой вес");

        // Валидация объема
        RuleFor(x => x.Volume)
            .NotNull().WithMessage("Введите объем")
            .GreaterThan(0).WithMessage("Объем должен быть больше нуля")
            .LessThanOrEqualTo(500).WithMessage("Указан слишком большой объем");

        // Валидация количества насосов
        RuleFor(x => x.Number)
            .GreaterThan(0).WithMessage("Количество насосов должно быть 1 или более")
            .LessThanOrEqualTo(1000).WithMessage("Нельзя рассчитать более 1000 насосов за раз");
    }
}