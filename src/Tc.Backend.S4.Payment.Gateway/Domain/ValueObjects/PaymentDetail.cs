using Tc.Backend.S4.Payment.Gateway.Domain.Fixed;

namespace Tc.Backend.S4.Payment.Gateway.Domain.ValueObjects;

public record PaymentDetail(PaymentStatus Status, PaymentType Type, decimal Amount);