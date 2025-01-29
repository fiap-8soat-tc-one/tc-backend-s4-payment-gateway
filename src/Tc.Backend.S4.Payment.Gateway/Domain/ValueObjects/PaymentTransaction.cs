namespace Tc.Backend.S4.Payment.Gateway.Domain.ValueObjects;

public record PaymentTransaction(string Number, string Document, string Message);