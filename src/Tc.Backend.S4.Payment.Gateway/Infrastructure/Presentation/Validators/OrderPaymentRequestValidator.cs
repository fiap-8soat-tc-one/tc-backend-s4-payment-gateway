using FluentValidation;
using Tc.Backend.S4.Payment.Gateway.Presentation.Requests;

namespace Tc.Backend.S4.Payment.Gateway.Presentation.Validators;

public class OrderPaymentRequestValidator : AbstractValidator<OrderPaymentRequest>
{
    public OrderPaymentRequestValidator()
    {
        RuleFor(x => x.TransactionDocument)
            .NotEmpty()
            .WithMessage("Transaction document is required.")
            .Matches(@"^\d{11}$")
            .WithMessage("Transaction document must be a valid CPF (11 digits).");

        RuleFor(x => x.TransactionMessage)
            .NotEmpty()
            .WithMessage("Transaction message is required.")
            .MaximumLength(255)
            .WithMessage("Transaction message cannot exceed 255 characters.");

        RuleFor(x => x.TransactionNumber)
            .NotEmpty()
            .WithMessage("Transaction number is required.");

        RuleFor(x => x.Total)
            .GreaterThan(0)
            .WithMessage("Total must be greater than zero.");

        RuleFor(x => x.PaymentStatus)
            .IsInEnum()
            .WithMessage("Invalid status value.");

        RuleFor(x => x.PaymentType)
            .IsInEnum()
            .WithMessage("Invalid payment type value.");
    }
}