using Tc.Backend.S4.Payment.Gateway.Domain.Fixed;

namespace Tc.Backend.S4.Payment.Gateway.Domain.Events;

public record OrderPaymentUpdatedEvent(PaymentStatus Status, string TransactionNumber);