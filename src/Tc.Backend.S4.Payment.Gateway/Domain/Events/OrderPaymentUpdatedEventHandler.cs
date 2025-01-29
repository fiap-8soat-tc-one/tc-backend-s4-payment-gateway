using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Tc.Backend.S4.Payment.Gateway.Domain.Contracts;
using Tc.Backend.S4.Payment.Gateway.Infrastructure.Common;

namespace Tc.Backend.S4.Payment.Gateway.Domain.Events;

public class OrderPaymentUpdatedEventHandler(IConnectionFactory busFactory) : IOrderPaymentUpdatedEventHandler
{
    private const string ExchangeName = "ha.tc-payment-backend-api.payment.event.updated";
    private const string ExchangeType = "topic";
    private const string RoutingKey = "tc-payment-backend-api.payment.event.updated";
    private const bool Durable = true;
    private const bool Mandatory = true;

    public async Task HandleAsync(OrderPaymentUpdatedEvent @event, CancellationToken cancellationToken)
    {
        await using var busConnection = await busFactory.CreateConnectionAsync(cancellationToken);

        await using var channel = await busConnection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(ExchangeName, ExchangeType, Durable, cancellationToken: cancellationToken);

        var message = MapMessageBody(@event);

        var body = Encoding.UTF8.GetBytes(message);

        var props = MapBasicProperties(@event);

        await channel.BasicPublishAsync(ExchangeName, RoutingKey, Mandatory, props, body, cancellationToken);
    }

    private string MapMessageBody(OrderPaymentUpdatedEvent @event)
    {
        return JsonSerializer.Serialize(new
        {
            CreationDate = DateTime.Now,
            Content = new[] { @event }
        }, JsonNamingMessagePolicy.JsonOptions);
    }

    private static BasicProperties MapBasicProperties(OrderPaymentUpdatedEvent @event)
    {
        return new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
            Headers = new Dictionary<string, object>  { { "tracking_id", $"order-payment-queue-id-{@event.TransactionNumber}" } }!
        };
    }
}