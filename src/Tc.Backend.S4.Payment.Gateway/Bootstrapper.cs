using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Http.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Tc.Backend.S4.Payment.Gateway.Application.UseCases;
using Tc.Backend.S4.Payment.Gateway.Domain.Contracts;
using Tc.Backend.S4.Payment.Gateway.Domain.Entities;
using Tc.Backend.S4.Payment.Gateway.Infrastructure.Persistence;

namespace Tc.Backend.S4.Payment.Gateway;

public static class Bootstrapper
{
    private static readonly List<IConvention> DefaultConventions =
    [
        new EnumRepresentationConvention(BsonType.String),
        new IgnoreExtraElementsConvention(true),
        new IgnoreIfNullConvention(true)
    ];

    public static IServiceCollection AddMongoContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDb");
        var mongoClient = new MongoClient(connectionString);

        BsonSerializer.RegisterSerializer(typeof(DateTime), DateTimeSerializer.LocalInstance);

        return services
            .AddSingleton<IMongoClient>(mongoClient)
            .AddSingleton<IOrderPaymentRepository, OrderPaymentRepository>()
            .AddSingleton<IConventionPack>(CreateConventionPack());
    }

    
    public static IServiceCollection AddApplicationContext(this IServiceCollection services)
    {
        return services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        })
            .AddOpenApi()
            .AddValidatorsFromAssemblyContaining<Program>()
            .AddScoped<IRegisterOrderPaymentUseCase, RegisterOrderPaymentUseCase>();
    }
    
    private static ConventionPack CreateConventionPack()
    {
        var conventionPack = new ConventionPack();
        conventionPack.AddRange(DefaultConventions.DistinctBy(convention => convention.Name));
        return conventionPack;
    }
}