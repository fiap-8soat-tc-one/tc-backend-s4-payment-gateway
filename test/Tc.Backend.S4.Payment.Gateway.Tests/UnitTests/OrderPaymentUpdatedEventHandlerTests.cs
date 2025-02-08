using Moq;
using RabbitMQ.Client;
using Tc.Backend.S4.Payment.Gateway.Domain.Events;
using Tc.Backend.S4.Payment.Gateway.Domain.Fixed;

namespace Tc.Backend.S4.Payment.Gateway.Tests.UnitTests;

public class OrderPaymentUpdatedEventHandlerTests
{
    [Fact]
    public async Task HandleAsync_Should_Publish_Message_Successfully()
    {
        var mockConnectionFactory = new Mock<IConnectionFactory>();
        var mockConnection = new Mock<IConnection>();
        var mockChannel = new Mock<IChannel>();

        mockConnectionFactory.Setup(f => f.CreateConnectionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockConnection.Object);
        mockConnection.Setup(c => c.CreateChannelAsync(It.IsAny<CreateChannelOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockChannel.Object);

        var handler = new OrderPaymentUpdatedEventHandler(mockConnectionFactory.Object);
        var @event = new OrderPaymentUpdatedEvent(PaymentStatus.Approved, "12345");

        await handler.HandleAsync(@event, CancellationToken.None);

        mockChannel.Verify(c => c.BasicPublishAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<BasicProperties>(),
            It.IsAny<ReadOnlyMemory<byte>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Should_Throw_Exception_When_Publish_Fails()
    {
        var mockConnectionFactory = new Mock<IConnectionFactory>();
        var mockConnection = new Mock<IConnection>();
        var mockChannel = new Mock<IChannel>();

        mockConnectionFactory.Setup(f => f.CreateConnectionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockConnection.Object);
        mockConnection.Setup(c => c.CreateChannelAsync(It.IsAny<CreateChannelOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Connection failed"));

        var handler = new OrderPaymentUpdatedEventHandler(mockConnectionFactory.Object);
        var @event = new OrderPaymentUpdatedEvent(PaymentStatus.Approved, "12345");

        await Assert.ThrowsAsync<Exception>(() => handler.HandleAsync(@event, CancellationToken.None));
    }
}