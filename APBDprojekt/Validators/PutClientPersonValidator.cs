using APBDprojekt.RequestModels;
using FluentValidation;

namespace APBDprojekt.Validators;

public class PutClientPersonValidator: AbstractValidator<PutClientPersonRequestModel>
{
    public PutClientPersonValidator()
    {
        RuleFor(e => e.Name).MaximumLength(50).When(e => !string.IsNullOrEmpty(e.Name));
        RuleFor(e => e.Surname).MaximumLength(50).When(e => !string.IsNullOrEmpty(e.Surname));
        RuleFor(e => e.TelephoneNumber).Length(9).Matches(@"^\d+$").When(e => !string.IsNullOrEmpty(e.TelephoneNumber));
        RuleFor(e => e.Email).EmailAddress().When(e => !string.IsNullOrEmpty(e.Email));
        RuleFor(e => e.Address).MaximumLength(50).When(e => !string.IsNullOrEmpty(e.Address));
    }
}