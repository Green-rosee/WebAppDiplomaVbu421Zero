using FluentValidation;

namespace WebAppEstimate.Models.User;

public class UserLoginValid : AbstractValidator<UserLogin>
{
    public UserLoginValid()
    {
        //-------проверяет валидацию
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Login обязателен для заполнения.")
            .MinimumLength(5).WithMessage("Login должен содержать как минимум 5 символов.");
        //------
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password обязателен для заполнения.")
            .MinimumLength(5).WithMessage("Password должен содержать как минимум 5 символов.");
    }
}