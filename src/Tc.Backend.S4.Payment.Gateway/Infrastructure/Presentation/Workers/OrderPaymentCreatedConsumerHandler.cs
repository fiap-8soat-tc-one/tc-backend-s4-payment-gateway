using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Tc.Backend.S4.Payment.Gateway.Domain.Contracts;
using Tc.Backend.S4.Payment.Gateway.Domain.Entities;
using Tc.Backend.S4.Payment.Gateway.Domain.Fixed;
using Tc.Backend.S4.Payment.Gateway.Domain.ValueObjects;

namespace Tc.Backend.S4.Payment.Gateway.Infrastructure.Presentation.Workers;

public class OrderPaymentCreatedConsumerHandler(IOrderPaymentRepository repository, IConnectionFactory busFactory)
    : BackgroundService
{
    private const string QueueName = "ha.tc-order-backend-api.order.event.order.created.queue";
    private const bool Durable = true;
    private const bool AutoAck = true;
    private const bool Exclusive = false;
    private const bool AutoDelete = false;

    private readonly Dictionary<string, object?> _arguments = new()
    {
        { "x-dead-letter-exchange", "ha.tc-order-backend-api.order.event.order.created.dlx" },
        { "x-dead-letter-routing-key", "tc-order-backend-api.order.event.order.created" },
        { "x-queue-type", "classic" }
    };


    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await using var busConnection = await busFactory.CreateConnectionAsync(cancellationToken);
            await using var channel = await busConnection.CreateChannelAsync(cancellationToken: cancellationToken);
            await channel.QueueDeclareAsync(QueueName, Durable, Exclusive, AutoDelete, _arguments,
                cancellationToken: cancellationToken);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, @event) =>
            {
                var message = Encoding.UTF8.GetString(@event.Body.ToArray());
                using var body = JsonDocument.Parse(message);
                var transactionNumber = body.RootElement.GetProperty("content")[0].GetProperty("id").GetString();
                if (!string.IsNullOrEmpty(transactionNumber))
                {
                    var transaction = new PaymentTransaction(transactionNumber, string.Empty, string.Empty);
                    var orderPayment = new OrderPayment(transaction, PaymentType.Undefined, PaymentStatus.Waiting, 0);
                    await repository.AddOrUpdateAsync(orderPayment, cancellationToken);
                }
            };

            await channel.BasicConsumeAsync(QueueName, AutoAck, consumer, cancellationToken);

            await Task.Delay(1000, cancellationToken);
        }
    }
}