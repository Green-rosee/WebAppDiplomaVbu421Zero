using FluentValidation;

namespace WebAppEstimate.Areas.SiteCerber.Models;

public class FlangeModelValidator : AbstractValidator<FlangeModel>
{
    public FlangeModelValidator()
    {
        RuleFor(x => x.OuterDiameter)
            .GreaterThan(0);

        RuleFor(x => x.InnerDiameter)
            .GreaterThan(0)
            .LessThan(x => x.OuterDiameter);

        RuleFor(x => x.Thickness)
            .GreaterThan(0);

        RuleFor(x => x.HoleCount)
            .GreaterThan(0);

        RuleFor(x => x.HoleDiameter)
            .GreaterThan(0);

        RuleFor(x => x.BoltCircleDiameter)
            .GreaterThan(x => x.InnerDiameter)
            .LessThan(x => x.OuterDiameter);
    }
}