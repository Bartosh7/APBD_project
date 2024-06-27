using APBDprojekt.RequestModels;
using FluentValidation;

namespace APBDprojekt.Validators;

public class PutClientCompanyValidator:AbstractValidator<PutClientCompanyRequestModel>
{
    public PutClientCompanyValidator() 
    {
        RuleFor(e => e.Name).MaximumLength(50).NotEmpty().When(e => !string.IsNullOrEmpty(e.Name));
        RuleFor(e => e.TelephoneNumber).Length(9).Matches(@"^\d+$").When(e => !string.IsNullOrEmpty(e.TelephoneNumber));
        RuleFor(e => e.Email).EmailAddress().MaximumLength(50).When(e => !string.IsNullOrEmpty(e.Email));
        RuleFor(e => e.Address).MaximumLength(50).When(e => !string.IsNullOrEmpty(e.Address));
        
    }
}