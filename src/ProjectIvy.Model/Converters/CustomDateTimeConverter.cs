using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectIvy.Model.Converters;

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    private const string AlternativeDateFormat = "yyyy-MM-ddTHH:mm:ss";
    private const string DateFormat = "yyyy-MM-dd HH:mm:ss";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string value = reader.GetString();
        try
        {
            return DateTime.ParseExact(value, DateFormat, CultureInfo.InvariantCulture);
        }
        catch
        {
            return DateTime.ParseExact(value, AlternativeDateFormat, CultureInfo.InvariantCulture);
        }
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}