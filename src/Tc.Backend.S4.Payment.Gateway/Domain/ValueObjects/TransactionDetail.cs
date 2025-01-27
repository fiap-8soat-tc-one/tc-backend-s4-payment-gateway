namespace Tc.Backend.S4.Payment.Gateway.Domain.ValueObjects;

public record TransactionDetail(string Number, string Document, string Message);