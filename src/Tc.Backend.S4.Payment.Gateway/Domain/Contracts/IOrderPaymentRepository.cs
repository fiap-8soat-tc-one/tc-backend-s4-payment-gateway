using Tc.Backend.S4.Payment.Gateway.Domain.Entities;

namespace Tc.Backend.S4.Payment.Gateway.Domain.Contracts;

public interface IOrderPaymentRepository
{
     Task AddAsync(OrderPayment entity, CancellationToken cancellationToken);
     Task<bool> ExistsAsync(string number, CancellationToken cancellationToken);
}