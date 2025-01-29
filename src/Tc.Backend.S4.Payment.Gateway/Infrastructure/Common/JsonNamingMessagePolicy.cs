using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tc.Backend.S4.Payment.Gateway.Infrastructure.Common;

public class JsonNamingMessagePolicy : JsonNamingPolicy
{
    public static JsonSerializerOptions JsonOptions = new()
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