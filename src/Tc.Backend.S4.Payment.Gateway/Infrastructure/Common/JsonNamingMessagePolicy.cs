using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tc.Backend.S4.Payment.Gateway.Infrastructure.Common;

[ExcludeFromDescription]
public class JsonNamingMessagePolicy : JsonNamingPolicy
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = SnakeCaseLower,
        Converters =
        {
            new JsonStringEnumConverter(new JsonNamingMessagePolicy())
        }
    };

    public override string ConvertName(string name)
    {
        return name.ToUpper();
    }
}