using System.Text.Json;
using System.Text.Json.Serialization;

public class DateFormatConverter : JsonConverter<DateTime>
{
    private readonly string _dateFormat;

    public DateFormatConverter()
    {
        _dateFormat = "yyyy-MM-dd";
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(_dateFormat));
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }

        var dateString = reader.GetString();
        if (DateTime.TryParseExact(dateString, _dateFormat, null, System.Globalization.DateTimeStyles.None, out var date))
        {
            return date;
        }

        throw new JsonException();
    }
}