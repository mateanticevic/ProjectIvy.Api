using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ProjectIvy.Model.Binding.Expense
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ExpenseSort
    {
        Date,
        Created,
        Modified,
        Amount
    }
}
