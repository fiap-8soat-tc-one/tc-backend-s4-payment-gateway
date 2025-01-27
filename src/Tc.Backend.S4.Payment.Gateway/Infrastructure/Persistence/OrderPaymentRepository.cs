using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Tc.Backend.S4.Payment.Gateway.Domain.Contracts;
using Tc.Backend.S4.Payment.Gateway.Domain.Entities;

namespace Tc.Backend.S4.Payment.Gateway.Infrastructure.Persistence;

public class OrderPaymentRepository : IOrderPaymentRepository
{
    private readonly IMongoCollection<OrderPayment> _collection;
    private const string Database = "Payment";

    public OrderPaymentRepository(IMongoClient client, IConventionPack conventionPack)
    {
        ConventionRegistry.Register("Convention", conventionPack, _ => true);

        _collection = client
            .GetDatabase(Database)
            .GetCollection<OrderPayment>(nameof(OrderPayment));
    }
    
    public virtual async Task AddAsync(OrderPayment entity, CancellationToken cancellationToken)
    {
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(string number, CancellationToken cancellationToken)
    {
       return await _collection.CountDocumentsAsync(x => x.TransactionDetail.Number == number, cancellationToken: cancellationToken) > 0;
    }
}