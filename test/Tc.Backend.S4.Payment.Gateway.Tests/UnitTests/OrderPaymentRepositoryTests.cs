using FluentAssertions;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Moq;
using Tc.Backend.S4.Payment.Gateway.Domain.Entities;
using Tc.Backend.S4.Payment.Gateway.Domain.Fixed;
using Tc.Backend.S4.Payment.Gateway.Domain.ValueObjects;
using Tc.Backend.S4.Payment.Gateway.Infrastructure.Persistence;

namespace Tc.Backend.S4.Payment.Gateway.Tests.UnitTests;

public class OrderPaymentRepositoryTests
{
    [Fact]
    public async Task AddOrUpdateAsync_Should_InsertOrUpdate_Successfully()
    {
        var mockCollection = new Mock<IMongoCollection<OrderPayment>>();
        var mockClient = new Mock<IMongoClient>();
        var mockDatabase = new Mock<IMongoDatabase>();

        mockClient.Setup(c => c.GetDatabase(It.IsAny<string>(), null)).Returns(mockDatabase.Object);
        mockDatabase.Setup(d => d.GetCollection<OrderPayment>(It.IsAny<string>(), null)).Returns(mockCollection.Object);

        var repository = new OrderPaymentRepository(mockClient.Object, Mock.Of<IConventionPack>());
        var orderPayment = new OrderPayment(new PaymentTransaction("12345", "12345678901", "Payment"),
            PaymentType.Credit, PaymentStatus.Approved, 200);

        await repository.AddOrUpdateAsync(orderPayment, CancellationToken.None);

        mockCollection.Verify(c => c.ReplaceOneAsync(
            It.IsAny<FilterDefinition<OrderPayment>>(),
            orderPayment,
            It.Is<ReplaceOptions>(o => o.IsUpsert),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddOrUpdateAsync_Should_Throw_Exception_When_Database_Fails()
    {
        var mockCollection = new Mock<IMongoCollection<OrderPayment>>();
        var mockClient = new Mock<IMongoClient>();
        var mockDatabase = new Mock<IMongoDatabase>();

        mockClient.Setup(c => c.GetDatabase(It.IsAny<string>(), null)).Returns(mockDatabase.Object);
        mockDatabase.Setup(d => d.GetCollection<OrderPayment>(It.IsAny<string>(), null)).Returns(mockCollection.Object);
        mockCollection.Setup(c => c.ReplaceOneAsync(
            It.IsAny<FilterDefinition<OrderPayment>>(),
            It.IsAny<OrderPayment>(),
            It.IsAny<ReplaceOptions>(),
            It.IsAny<CancellationToken>())).ThrowsAsync(new MongoException("Database error"));

        var repository = new OrderPaymentRepository(mockClient.Object, Mock.Of<IConventionPack>());
        var orderPayment = new OrderPayment(new PaymentTransaction("12345", "12345678901", "Payment"),
            PaymentType.Credit, PaymentStatus.Approved, 200);

        await Assert.ThrowsAsync<MongoException>(
            () => repository.AddOrUpdateAsync(orderPayment, CancellationToken.None));
    }

    [Fact]
    public async Task GetByTransactionNumberAsync_Should_Return_OrderPayment_When_Found()
    {
        var mockCollection = new Mock<IMongoCollection<OrderPayment>>();
        var mockClient = new Mock<IMongoClient>();
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCursor = new Mock<IAsyncCursor<OrderPayment>>();

        var expectedOrder = new OrderPayment(new PaymentTransaction("12345", "12345678901", "Payment"),
            PaymentType.Credit, PaymentStatus.Approved, 200);

        mockClient.Setup(c => c.GetDatabase(It.IsAny<string>(), null)).Returns(mockDatabase.Object);
        mockDatabase.Setup(d => d.GetCollection<OrderPayment>(It.IsAny<string>(), null)).Returns(mockCollection.Object);
        mockCollection.Setup(c => c.FindAsync(
            It.IsAny<FilterDefinition<OrderPayment>>(),
            It.IsAny<FindOptions<OrderPayment, OrderPayment>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(mockCursor.Object);

        mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        mockCursor.Setup(c => c.Current).Returns(new List<OrderPayment> { expectedOrder });

        var repository = new OrderPaymentRepository(mockClient.Object, Mock.Of<IConventionPack>());
        var result = await repository.GetByTransactionNumberAsync("12345", CancellationToken.None);

        result.Should().Be(expectedOrder);
    }

    [Fact]
    public async Task GetByTransactionNumberAsync_Should_Return_Null_When_Not_Found()
    {
        var mockCollection = new Mock<IMongoCollection<OrderPayment>>();
        var mockClient = new Mock<IMongoClient>();
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCursor = new Mock<IAsyncCursor<OrderPayment>>();

        mockClient.Setup(c => c.GetDatabase(It.IsAny<string>(), null)).Returns(mockDatabase.Object);
        mockDatabase.Setup(d => d.GetCollection<OrderPayment>(It.IsAny<string>(), null)).Returns(mockCollection.Object);
        mockCollection.Setup(c => c.FindAsync(
            It.IsAny<FilterDefinition<OrderPayment>>(),
            It.IsAny<FindOptions<OrderPayment, OrderPayment>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(mockCursor.Object);

        mockCursor.Setup(c => c.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var repository = new OrderPaymentRepository(mockClient.Object, Mock.Of<IConventionPack>());
        var result = await repository.GetByTransactionNumberAsync("67890", CancellationToken.None);

        result.Should().BeNull();
    }
}