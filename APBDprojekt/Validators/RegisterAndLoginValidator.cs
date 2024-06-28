using APBDprojekt.RequestModels;
using FluentValidation;

namespace APBDprojekt.Validators;

public class RegisterAndLoginValidator:AbstractValidator<RegisterAndLoginRequestModel>
{
    public RegisterAndLoginValidator()
    {
        RuleFor(e => e.Password).NotEmpty();
        RuleFor(e => e.UserName).NotEmpty();
    }
}