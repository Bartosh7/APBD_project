using APBDprojekt.RequestModels;
using FluentValidation;

namespace APBDprojekt.Validators;

public class PostClientCompanyValidator : AbstractValidator<PostClientCompanyRequestModel>
{
    public PostClientCompanyValidator()
    {
        RuleFor(e => e.Name).MaximumLength(50).NotEmpty();
        RuleFor(e => e.KRS).Must(krs => krs.Length is 9 or 14).Matches(@"^\d+$");
        RuleFor(e => e.TelephoneNumber).Length(9).Matches(@"^\d+$");
        RuleFor(e => e.Email).EmailAddress().NotEmpty().MaximumLength(50);
        RuleFor(e => e.Address).MaximumLength(50).NotEmpty();

    }
}