using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using Tc.Backend.S4.Payment.Gateway;
using Tc.Backend.S4.Payment.Gateway.Domain.Contracts;
using Tc.Backend.S4.Payment.Gateway.Presentation.Requests;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationContext();
builder.Services.AddMongoContext(builder.Configuration);

var app = builder.Build();

app.MapPost("api/public/v1/hook/orders/payments", async (
   [FromBody] OrderPaymentRequest request,
   [FromServices] IValidator<OrderPaymentRequest> validator,
   [FromServices] IRegisterOrderPaymentUseCase useCase,
    CancellationToken cancellationToken) =>
{
    var validatorHandler = await validator.ValidateAsync(request);

    if (!validatorHandler.IsValid)
        return Results.UnprocessableEntity(new { errors = validatorHandler.Errors, content = request });

    await useCase.HandleAsync(request, cancellationToken);
    return Results.Accepted();
    
})
.WithSummary("Webhook Register Order Payment")
.WithDescription("(Public Endpoint) This endpoint is responsible for receiving the payment parameters.")
.WithName("ReceivePaymentWebhook");

app.MapOpenApi();
app.MapScalarApiReference("/swagger",options => 
{
    options.WithTitle("tc-backend-s4-payment-gateway");
    options.WithSidebar(false);	
});

app.Run();