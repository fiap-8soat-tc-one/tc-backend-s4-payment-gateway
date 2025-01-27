using Tc.Backend.S4.Payment.Gateway.Domain.Fixed;

namespace Tc.Backend.S4.Payment.Gateway.Presentation.Requests;

public record OrderPaymentRequest(
    PaymentType PaymentType,
    PaymentStatus PaymentStatus,
    decimal Total,
    string TransactionDocument,
    string TransactionMessage,
    string TransactionNumber);