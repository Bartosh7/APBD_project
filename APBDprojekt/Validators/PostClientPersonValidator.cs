using APBDprojekt.RequestModels;
using FluentValidation;

namespace APBDprojekt.Validators;

public class PostClientPersonValidator : AbstractValidator<PostClientPersonRequestModel>
{
    public PostClientPersonValidator()
    {
        RuleFor(e => e.Name).MaximumLength(50).NotEmpty();
        RuleFor(e => e.Surname).MaximumLength(50).NotEmpty();
        RuleFor(e => e.PeselNumber).Length(11).Matches(@"^\d+$");
        RuleFor(e => e.TelephoneNumber).Length(9).Matches(@"^\d+$");
        RuleFor(e => e.Email).EmailAddress().NotEmpty();
        RuleFor(e => e.Address).MaximumLength(50).NotEmpty();
        
    }
}