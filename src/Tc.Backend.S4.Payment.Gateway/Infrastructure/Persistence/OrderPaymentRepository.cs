using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Tc.Backend.S4.Payment.Gateway.Domain.Contracts;
using Tc.Backend.S4.Payment.Gateway.Domain.Entities;

namespace Tc.Backend.S4.Payment.Gateway.Infrastructure.Persistence;

public sealed class OrderPaymentRepository : IOrderPaymentRepository
{
    private const string Database = "Payment";
    private readonly IMongoCollection<OrderPayment> _collection;

    public OrderPaymentRepository(IMongoClient client, IConventionPack conventionPack)
    {
        ConventionRegistry.Register("Convention", conventionPack, _ => true);

        _collection = client
            .GetDatabase(Database)
            .GetCollection<OrderPayment>(nameof(OrderPayment));
    }

    public async Task AddOrUpdateAsync(OrderPayment entity, CancellationToken cancellationToken)
    {
        await _collection.ReplaceOneAsync(x => x.Transaction.Number == entity.Transaction.Number, entity, new ReplaceOptions { IsUpsert = true }, cancellationToken);
    }

    public async Task<OrderPayment?> GetByTransactionNumberAsync(string number, CancellationToken cancellationToken)
    {
        return await _collection.Find(x => x.Transaction.Number == number).FirstOrDefaultAsync(cancellationToken);
    }
}