using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace ProjectIvy.Model.Binding.ExpenseType;

[JsonConverter(typeof(StringEnumConverter))]
public enum ExpenseTypeSort
{
	Name,
	Top10
}
