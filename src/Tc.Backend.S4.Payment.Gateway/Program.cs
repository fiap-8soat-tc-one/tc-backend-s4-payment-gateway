using Carter;
using Tc.Backend.S4.Payment.Gateway;
using Tc.Backend.S4.Payment.Gateway.Infrastructure.Presentation.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationContext();
builder.Services.AddMongoContext(builder.Configuration);
builder.Services.AddRabbitMqContext(builder.Configuration);
builder.Services.AddCarter();
builder.Services.AddHostedService<OrderPaymentCreatedConsumerHandler>();

var app = builder.Build();

app.MapCarter();
app.MapOpenApiWithScalarReference();

app.Run();