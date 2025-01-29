using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Tc.Backend.S4.Payment.Gateway.Domain.Fixed;
using Tc.Backend.S4.Payment.Gateway.Domain.ValueObjects;

namespace Tc.Backend.S4.Payment.Gateway.Domain.Entities;

public class OrderPayment
{
    public OrderPayment() { }

    public OrderPayment(PaymentTransaction transaction, PaymentType type, PaymentStatus status, decimal amount)
    {
        Id = ObjectId.GenerateNewId().ToString();
        Transaction = transaction;
        Type = type;
        Status = status;
        Amount = amount;
        CreatedAt = DateTime.Now;
        UpdatedAt = CreatedAt;
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public  string Id { get; private set; }

    public  PaymentTransaction Transaction { get; private set; }

    public PaymentStatus Status { get; private set; }

    public PaymentType Type { get; private set; }

    public decimal Amount { get; private set; }

    public DateTime CreatedAt { get; }

    [BsonRepresentation(BsonType.DateTime)]
    public DateTime UpdatedAt { get; private set; }

    public void SetDetails(PaymentStatus status, PaymentType type, decimal amount)
    {
        Type = type;
        Status = status;
        Amount = amount;
    }

    public void SetTransaction(PaymentTransaction transaction)
    {
        Transaction = transaction;
    }

    public void OrderPaymentChanged()
    {
        UpdatedAt = DateTime.Now;
    }
}