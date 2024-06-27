using APBDprojekt.RequestModels;
using FluentValidation;

namespace APBDprojekt.Validators;

public class PostContractPaymentValidator : AbstractValidator<PostContractPaymentRequestModel>
{
    public PostContractPaymentValidator()
    {
        RuleFor(e => e.AmountOfPayment).GreaterThan(0);
    }
}