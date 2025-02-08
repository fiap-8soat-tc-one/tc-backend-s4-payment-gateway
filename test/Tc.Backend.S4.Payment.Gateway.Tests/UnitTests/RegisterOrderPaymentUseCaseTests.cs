using MongoDB.Driver;
using Moq;
using Tc.Backend.S4.Payment.Gateway.Application.UseCases;
using Tc.Backend.S4.Payment.Gateway.Domain.Contracts;
using Tc.Backend.S4.Payment.Gateway.Domain.Entities;
using Tc.Backend.S4.Payment.Gateway.Domain.Events;
using Tc.Backend.S4.Payment.Gateway.Domain.Fixed;
using Tc.Backend.S4.Payment.Gateway.Domain.ValueObjects;
using Tc.Backend.S4.Payment.Gateway.Infrastructure.Presentation.Requests;

namespace Tc.Backend.S4.Payment.Gateway.Tests.UnitTests;

public class RegisterOrderPaymentUseCaseTests
{
    [Fact]
    public async Task HandleAsync_Should_Update_OrderPayment_And_Trigger_Event_Successfully()
    {
        var mockRepository = new Mock<IOrderPaymentRepository>();
        var mockEventHandler = new Mock<IOrderPaymentUpdatedEventHandler>();

        var request = new OrderPaymentRequest(PaymentType.Credit, PaymentStatus.Approved, 100, "12345678901",
            "Payment processed", "12345");
        var existingOrderPayment = new OrderPayment(new PaymentTransaction("12345", "12345678901", "Initial payment"),
            PaymentType.Debit, PaymentStatus.Waiting, 50);

        mockRepository.Setup(r =>
                r.GetByTransactionNumberAsync(request.TransactionNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOrderPayment);

        var useCase = new RegisterOrderPaymentUseCase(mockRepository.Object, mockEventHandler.Object);

        await useCase.HandleAsync(request, CancellationToken.None);

        mockRepository.Verify(r => r.AddOrUpdateAsync(existingOrderPayment, It.IsAny<CancellationToken>()), Times.Once);
        mockEventHandler.Verify(e => e.HandleAsync(
            It.Is<OrderPaymentUpdatedEvent>(ev =>
                ev.Status == request.PaymentStatus && ev.TransactionNumber == request.TransactionNumber),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Should_Not_Trigger_Event_When_OrderPayment_Not_Found()
    {
        var mockRepository = new Mock<IOrderPaymentRepository>();
        var mockEventHandler = new Mock<IOrderPaymentUpdatedEventHandler>();

        var request = new OrderPaymentRequest(PaymentType.Credit, PaymentStatus.Approved, 100, "12345678901",
            "Payment processed", "12345");

        mockRepository.Setup(r =>
                r.GetByTransactionNumberAsync(request.TransactionNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderPayment)null);

        var useCase = new RegisterOrderPaymentUseCase(mockRepository.Object, mockEventHandler.Object);

        await useCase.HandleAsync(request, CancellationToken.None);

        mockRepository.Verify(r => r.AddOrUpdateAsync(It.IsAny<OrderPayment>(), It.IsAny<CancellationToken>()),
            Times.Never);
        mockEventHandler.Verify(e => e.HandleAsync(It.IsAny<OrderPaymentUpdatedEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_Should_Throw_Exception_When_Repository_Fails()
    {
        var mockRepository = new Mock<IOrderPaymentRepository>();
        var mockEventHandler = new Mock<IOrderPaymentUpdatedEventHandler>();

        var request = new OrderPaymentRequest(PaymentType.Credit, PaymentStatus.Approved, 100, "12345678901",
            "Payment processed", "12345");
        var existingOrderPayment = new OrderPayment(new PaymentTransaction("12345", "12345678901", "Initial payment"),
            PaymentType.Debit, PaymentStatus.Waiting, 50);

        mockRepository.Setup(r =>
                r.GetByTransactionNumberAsync(request.TransactionNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOrderPayment);

        mockRepository.Setup(r => r.AddOrUpdateAsync(It.IsAny<OrderPayment>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new MongoException("Database error"));

        var useCase = new RegisterOrderPaymentUseCase(mockRepository.Object, mockEventHandler.Object);

        await Assert.ThrowsAsync<MongoException>(() => useCase.HandleAsync(request, CancellationToken.None));

        mockEventHandler.Verify(e => e.HandleAsync(It.IsAny<OrderPaymentUpdatedEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_Should_Throw_Exception_When_EventBus_Fails()
    {
        var mockRepository = new Mock<IOrderPaymentRepository>();
        var mockEventHandler = new Mock<IOrderPaymentUpdatedEventHandler>();

        var request = new OrderPaymentRequest(PaymentType.Credit, PaymentStatus.Approved, 100, "12345678901",
            "Payment processed", "12345");
        var existingOrderPayment = new OrderPayment(new PaymentTransaction("12345", "12345678901", "Initial payment"),
            PaymentType.Debit, PaymentStatus.Waiting, 50);

        mockRepository.Setup(r =>
                r.GetByTransactionNumberAsync(request.TransactionNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOrderPayment);

        mockEventHandler.Setup(e => e.HandleAsync(It.IsAny<OrderPaymentUpdatedEvent>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Event bus error"));

        var useCase = new RegisterOrderPaymentUseCase(mockRepository.Object, mockEventHandler.Object);

        await Assert.ThrowsAsync<Exception>(() => useCase.HandleAsync(request, CancellationToken.None));
    }
}