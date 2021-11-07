using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ProjectIvy.Business.Constants
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionSource
    {
        Hac,
        OtpBank,
        Revolut,
    }
}
