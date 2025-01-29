using Tc.Backend.S4.Payment.Gateway.Domain.Events;

namespace Tc.Backend.S4.Payment.Gateway.Domain.Contracts;

public interface IOrderPaymentUpdatedEventHandler
{
    Task HandleAsync(OrderPaymentUpdatedEvent @event, CancellationToken cancellationToken);
}