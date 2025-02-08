using Tc.Backend.S4.Payment.Gateway.Infrastructure.Presentation.Requests;

namespace Tc.Backend.S4.Payment.Gateway.Domain.Contracts;

public interface IRegisterOrderPaymentUseCase
{
    Task HandleAsync(OrderPaymentRequest request, CancellationToken cancellationToken);
}