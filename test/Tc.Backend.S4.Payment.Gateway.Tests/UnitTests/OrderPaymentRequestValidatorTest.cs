using FluentValidation.TestHelper;
using Tc.Backend.S4.Payment.Gateway.Domain.Fixed;
using Tc.Backend.S4.Payment.Gateway.Infrastructure.Presentation.Requests;
using Tc.Backend.S4.Payment.Gateway.Infrastructure.Presentation.Validators;

namespace Tc.Backend.S4.Payment.Gateway.Tests.UnitTests;

public class OrderPaymentRequestValidatorTests
{
    private readonly OrderPaymentRequestValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_TransactionDocument_Is_Empty()
    {
        var request =
            new OrderPaymentRequest(PaymentType.Credit, PaymentStatus.Waiting, 100, "", "Valid Message", "12345");

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.TransactionDocument)
            .WithErrorMessage("Transaction document is required.");
    }

    [Fact]
    public void Should_Have_Error_When_TransactionDocument_Is_Invalid()
    {
        var request = new OrderPaymentRequest(PaymentType.Credit, PaymentStatus.Waiting, 100, "123456", "Valid Message",
            "12345");

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.TransactionDocument)
            .WithErrorMessage("Transaction document must be a valid CPF (11 digits).");
    }

    [Fact]
    public void Should_Not_Have_Error_When_TransactionDocument_Is_Valid()
    {
        var request = new OrderPaymentRequest(PaymentType.Credit, PaymentStatus.Waiting, 100, "12345678901",
            "Valid Message", "12345");

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.TransactionDocument);
    }

    [Fact]
    public void Should_Have_Error_When_TransactionMessage_Is_Empty()
    {
        var request =
            new OrderPaymentRequest(PaymentType.Credit, PaymentStatus.Waiting, 100, "12345678901", "", "12345");

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.TransactionMessage)
            .WithErrorMessage("Transaction message is required.");
    }

    [Fact]
    public void Should_Have_Error_When_TransactionMessage_Exceeds_MaxLength()
    {
        var request = new OrderPaymentRequest(PaymentType.Credit, PaymentStatus.Waiting, 100, "12345678901",
            new string('a', 256), "12345");

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.TransactionMessage)
            .WithErrorMessage("Transaction message cannot exceed 255 characters.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_TransactionMessage_Is_Valid()
    {
        var request = new OrderPaymentRequest(PaymentType.Credit, PaymentStatus.Waiting, 100, "12345678901",
            "Valid Message", "12345");

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.TransactionMessage);
    }

    [Fact]
    public void Should_Have_Error_When_TransactionNumber_Is_Empty()
    {
        var request = new OrderPaymentRequest(PaymentType.Credit, PaymentStatus.Waiting, 100, "12345678901",
            "Valid Message", "");

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.TransactionNumber)
            .WithErrorMessage("Transaction number is required.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_TransactionNumber_Is_Valid()
    {
        var request = new OrderPaymentRequest(PaymentType.Credit, PaymentStatus.Waiting, 100, "12345678901",
            "Valid Message", "12345");

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.TransactionNumber);
    }

    [Fact]
    public void Should_Have_Error_When_Total_Is_Zero()
    {
        var request = new OrderPaymentRequest(PaymentType.Credit, PaymentStatus.Waiting, 0, "12345678901",
            "Valid Message", "12345");

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Total)
            .WithErrorMessage("Total must be greater than zero.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Total_Is_Greater_Than_Zero()
    {
        var request = new OrderPaymentRequest(PaymentType.Credit, PaymentStatus.Waiting, 100, "12345678901",
            "Valid Message", "12345");

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Total);
    }

    [Fact]
    public void Should_Have_Error_When_PaymentStatus_Is_Invalid()
    {
        var request = new OrderPaymentRequest(PaymentType.Credit, (PaymentStatus)99, 100, "12345678901",
            "Valid Message", "12345");

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.PaymentStatus)
            .WithErrorMessage("Invalid status value.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_PaymentStatus_Is_Valid()
    {
        var request = new OrderPaymentRequest(PaymentType.Credit, PaymentStatus.Approved, 100, "12345678901",
            "Valid Message", "12345");

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.PaymentStatus);
    }

    [Fact]
    public void Should_Have_Error_When_PaymentType_Is_Invalid()
    {
        var request = new OrderPaymentRequest((PaymentType)100, PaymentStatus.Waiting, 100, "12345678901",
            "Valid Message", "12345");

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.PaymentType)
            .WithErrorMessage("Invalid payment type value.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_PaymentType_Is_Valid()
    {
        var request = new OrderPaymentRequest(PaymentType.Pix, PaymentStatus.Waiting, 100, "12345678901",
            "Valid Message", "12345");

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.PaymentType);
    }
}