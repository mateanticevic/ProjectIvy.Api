using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ProjectIvy.Model.Binding.Inventory;

[JsonConverter(typeof(StringEnumConverter))]
public enum InventoryItemSort
{
    Name
}
