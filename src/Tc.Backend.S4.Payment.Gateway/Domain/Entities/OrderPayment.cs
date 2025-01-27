using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Tc.Backend.S4.Payment.Gateway.Domain.ValueObjects;

namespace Tc.Backend.S4.Payment.Gateway.Domain.Entities;

public class OrderPayment
{
    public OrderPayment(TransactionDetail transactionDetail, PaymentDetail payment)
    {
        Id = ObjectId.GenerateNewId().ToString();
        TransactionDetail = transactionDetail;
        Payment = payment;
        CreatedAt = DateTime.Now;
        UpdatedAt = CreatedAt;
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private set; }

    public TransactionDetail TransactionDetail { get; private set; }
    public PaymentDetail Payment { get; private set; }
    
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime CreatedAt { get; private set; }
    
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime UpdatedAt { get; private set; }
}