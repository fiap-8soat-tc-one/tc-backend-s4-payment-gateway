using Tc.Backend.S4.Payment.Gateway.Domain.Entities;

namespace Tc.Backend.S4.Payment.Gateway.Domain.Contracts;

public interface IOrderPaymentRepository
{
    Task AddOrUpdateAsync(OrderPayment entity, CancellationToken cancellationToken);
    Task<OrderPayment?> GetByTransactionNumberAsync(string number, CancellationToken cancellationToken);
}