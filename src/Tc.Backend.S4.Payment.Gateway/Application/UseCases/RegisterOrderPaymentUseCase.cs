using FluentValidation;
using Tc.Backend.S4.Payment.Gateway.Domain.Contracts;
using Tc.Backend.S4.Payment.Gateway.Domain.Entities;
using Tc.Backend.S4.Payment.Gateway.Domain.ValueObjects;
using Tc.Backend.S4.Payment.Gateway.Presentation.Requests;

namespace Tc.Backend.S4.Payment.Gateway.Application.UseCases;

public class RegisterOrderPaymentUseCase(
    IOrderPaymentRepository repository)
    : IRegisterOrderPaymentUseCase
{
    public async Task HandleAsync(OrderPaymentRequest request, CancellationToken cancellationToken)
    {
        var transactionExists = await repository.ExistsAsync(request.TransactionNumber, cancellationToken);

        if (!transactionExists)
        {
            var transactionDetail = new TransactionDetail(request.TransactionNumber, request.TransactionDocument, request.TransactionMessage);
            var paymentDetail = new PaymentDetail(request.PaymentStatus, request.PaymentType, request.Total);
            var orderPayment = new OrderPayment(transactionDetail, paymentDetail);

            await repository.AddAsync(orderPayment, cancellationToken);
        }
    }
}