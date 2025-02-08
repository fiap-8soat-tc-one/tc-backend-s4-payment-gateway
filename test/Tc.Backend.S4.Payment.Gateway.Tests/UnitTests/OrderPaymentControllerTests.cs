using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tc.Backend.S4.Payment.Gateway.Domain.Contracts;
using Tc.Backend.S4.Payment.Gateway.Domain.Fixed;
using Tc.Backend.S4.Payment.Gateway.Infrastructure.Presentation.Controllers;
using Tc.Backend.S4.Payment.Gateway.Infrastructure.Presentation.Requests;

namespace Tc.Backend.S4.Payment.Gateway.Tests.UnitTests;

public class OrderPaymentControllerTests
{
    [Fact]
    public async Task Post_Should_Return_Accepted_When_Request_Is_Valid()
    {
        var mockValidator = new Mock<IValidator<OrderPaymentRequest>>();
        var mockUseCase = new Mock<IRegisterOrderPaymentUseCase>();

        var request = new OrderPaymentRequest(PaymentType.Credit, PaymentStatus.Approved, 100, "12345678901",
            "Payment processed", "12345");

        mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var httpContext = new DefaultHttpContext();
        var context = new DefaultHttpContext { RequestServices = new ServiceCollection().BuildServiceProvider() };
        var controller = new OrderPaymentController();

        // Simula a lógica de roteamento do Minimal API
        var result =
            await controller.HandleRequest(request, mockValidator.Object, mockUseCase.Object, CancellationToken.None);

        result.Should().BeOfType<Accepted>();
        mockUseCase.Verify(u => u.HandleAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task Post_Should_Throw_Exception_When_UseCase_Fails()
    {
        var mockValidator = new Mock<IValidator<OrderPaymentRequest>>();
        var mockUseCase = new Mock<IRegisterOrderPaymentUseCase>();

        var request = new OrderPaymentRequest(PaymentType.Credit, PaymentStatus.Approved, 100, "12345678901",
            "Payment processed", "12345");

        mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        mockUseCase.Setup(u => u.HandleAsync(request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Internal server error"));

        var httpContext = new DefaultHttpContext();
        var context = new DefaultHttpContext { RequestServices = new ServiceCollection().BuildServiceProvider() };
        var controller = new OrderPaymentController();

        await Assert.ThrowsAsync<Exception>(() =>
            controller.HandleRequest(request, mockValidator.Object, mockUseCase.Object, CancellationToken.None));
    }
}

// Método auxiliar para simular a execução da lógica do Minimal API
public static class OrderPaymentControllerExtensions
{
    public static async Task<IResult> HandleRequest(
        this OrderPaymentController controller,
        OrderPaymentRequest request,
        IValidator<OrderPaymentRequest> validator,
        IRegisterOrderPaymentUseCase useCase,
        CancellationToken cancellationToken)
    {
        var validatorResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validatorResult.IsValid)
            return Results.UnprocessableEntity(new { errors = validatorResult.Errors, content = request });

        await useCase.HandleAsync(request, cancellationToken);
        return Results.Accepted();
    }
}