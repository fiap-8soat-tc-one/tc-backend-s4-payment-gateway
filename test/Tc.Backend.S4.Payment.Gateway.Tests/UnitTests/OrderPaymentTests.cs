using FluentAssertions;
using Tc.Backend.S4.Payment.Gateway.Domain.Entities;
using Tc.Backend.S4.Payment.Gateway.Domain.Fixed;
using Tc.Backend.S4.Payment.Gateway.Domain.ValueObjects;

namespace Tc.Backend.S4.Payment.Gateway.Tests.UnitTests;

public class OrderPaymentTests
{
    [Fact]
    public void Should_Update_OrderPayment_Status()
    {
        var transaction = new PaymentTransaction("12345", "12345678901", "Payment for order #001");
        var orderPayment = new OrderPayment(transaction, PaymentType.Debit, PaymentStatus.Waiting, 150.00m);

        orderPayment.SetDetails(PaymentStatus.Approved, PaymentType.Pix, 200.00m);

        orderPayment.Status.Should().Be(PaymentStatus.Approved);
    }

    [Fact]
    public void Should_Update_OrderPayment_Type()
    {
        var transaction = new PaymentTransaction("12345", "12345678901", "Payment for order #001");
        var orderPayment = new OrderPayment(transaction, PaymentType.Debit, PaymentStatus.Waiting, 150.00m);

        orderPayment.SetDetails(PaymentStatus.Approved, PaymentType.Pix, 200.00m);

        orderPayment.Type.Should().Be(PaymentType.Pix);
    }

    [Fact]
    public void Should_Update_OrderPayment_Amount()
    {
        var transaction = new PaymentTransaction("12345", "12345678901", "Payment for order #001");
        var orderPayment = new OrderPayment(transaction, PaymentType.Debit, PaymentStatus.Waiting, 150.00m);

        orderPayment.SetDetails(PaymentStatus.Approved, PaymentType.Pix, 200.00m);

        orderPayment.Amount.Should().Be(200.00m);
    }

    [Fact]
    public void Should_Update_Transaction()
    {
        var initialTransaction = new PaymentTransaction("12345", "12345678901", "Initial payment");
        var updatedTransaction = new PaymentTransaction("67890", "10987654321", "Updated payment");
        var orderPayment = new OrderPayment(initialTransaction, PaymentType.Credit, PaymentStatus.Approved, 300.00m);

        orderPayment.SetTransaction(updatedTransaction);

        orderPayment.Transaction.Should().Be(updatedTransaction);
    }

    [Fact]
    public void Should_Update_UpdatedAt_When_OrderPaymentChanged()
    {
        var transaction = new PaymentTransaction("12345", "12345678901", "Initial payment");
        var orderPayment = new OrderPayment(transaction, PaymentType.Debit, PaymentStatus.Waiting, 150.00m);

        var initialUpdatedAt = orderPayment.UpdatedAt;

        Thread.Sleep(1000);
        orderPayment.OrderPaymentChanged();

        orderPayment.UpdatedAt.Should().BeAfter(initialUpdatedAt);
    }
}