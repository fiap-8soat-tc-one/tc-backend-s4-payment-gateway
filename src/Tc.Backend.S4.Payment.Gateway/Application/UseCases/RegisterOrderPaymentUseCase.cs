using Tc.Backend.S4.Payment.Gateway.Domain.Contracts;
using Tc.Backend.S4.Payment.Gateway.Domain.Events;
using Tc.Backend.S4.Payment.Gateway.Domain.ValueObjects;
using Tc.Backend.S4.Payment.Gateway.Presentation.Requests;

namespace Tc.Backend.S4.Payment.Gateway.Application.UseCases;

public class RegisterOrderPaymentUseCase(
    IOrderPaymentRepository repository,
    IOrderPaymentUpdatedEventHandler eventBus)
    : IRegisterOrderPaymentUseCase
{
    public async Task HandleAsync(OrderPaymentRequest request, CancellationToken cancellationToken)
    {
        var orderPayment = await repository.GetByTransactionNumberAsync(request.TransactionNumber, cancellationToken);

        if (orderPayment != null)
        {
            var transactionDetail = new PaymentTransaction(request.TransactionNumber, request.TransactionDocument,
                request.TransactionMessage);

            orderPayment.SetDetails(request.PaymentStatus, request.PaymentType, request.Total);
            orderPayment.SetTransaction(transactionDetail);
            orderPayment.OrderPaymentChanged();

            await repository.AddOrUpdateAsync(orderPayment, cancellationToken);
            await eventBus.HandleAsync(new OrderPaymentUpdatedEvent(request.PaymentStatus, request.TransactionNumber),
                cancellationToken);
        }
    }
}