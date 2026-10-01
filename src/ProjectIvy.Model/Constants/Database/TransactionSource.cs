using System.Text.Json.Serialization;

namespace ProjectIvy.Model.Constants.Database;

[JsonConverter(typeof(JsonStringEnumConverter))]
[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum TransactionSource
{
    Hac,
    OtpBank,
    Revolut,
    ZabaBank,
}
