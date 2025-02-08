using Tc.Backend.S4.Payment.Gateway.Domain.Fixed;

namespace Tc.Backend.S4.Payment.Gateway.Infrastructure.Presentation.Requests;

public sealed record OrderPaymentRequest(
    PaymentType PaymentType,
    PaymentStatus PaymentStatus,
    decimal Total,
    string TransactionDocument,
    string TransactionMessage,
    string TransactionNumber);