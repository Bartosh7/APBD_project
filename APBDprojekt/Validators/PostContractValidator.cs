using APBDprojekt.RequestModels;
using FluentValidation;
using System;

namespace APBDprojekt.Validators
{
    public class PostContractValidator : AbstractValidator<PostContractModelRequestModel>
    {
        public PostContractValidator()
        {
            RuleFor(e => e.PaymentStartTime)
                .NotEmpty();

            RuleFor(e => e.PaymentEndTime)
                .NotEmpty()
                .Must((model, endTime) => BeWithinValidRange(model.PaymentStartTime, endTime))
                .WithMessage("Payment end time must be at least 3 days and no more than 30 days after payment start time");

            RuleFor(e => e.YearsOfVersionSupport)
                .NotEmpty()
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(4)
                .WithMessage("Years of version support must be no more than 4");
        }

        private bool BeWithinValidRange(DateTime startTime, DateTime endTime)
        {
            var daysDifference = (endTime - startTime).TotalDays;
            return daysDifference >= 3 && daysDifference <= 30;
        }
    }
}