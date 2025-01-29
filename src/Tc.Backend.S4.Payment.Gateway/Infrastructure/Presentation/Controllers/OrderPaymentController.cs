using Carter;
using FluentValidation;
using Tc.Backend.S4.Payment.Gateway.Domain.Contracts;
using Tc.Backend.S4.Payment.Gateway.Presentation.Requests;

namespace Tc.Backend.S4.Payment.Gateway.Infrastructure.Presentation.Controllers;

public class OrderPaymentController : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/public/v1/hook/orders/payments", async (
                OrderPaymentRequest request,
                IValidator<OrderPaymentRequest> validator,
                IRegisterOrderPaymentUseCase useCase,
                CancellationToken cancellationToken) =>
            {
                var validatorHandler = await validator.ValidateAsync(request, cancellationToken);

                if (!validatorHandler.IsValid)
                    return Results.UnprocessableEntity(new { errors = validatorHandler.Errors, content = request });

                await useCase.HandleAsync(request, cancellationToken);
                return Results.Accepted();
            })
            .WithSummary("Webhook Register Order Payment")
            .WithDescription("(Public Endpoint) This endpoint is responsible for receiving the payment parameters.")
            .WithName("ReceivePaymentWebhook");
    }
}