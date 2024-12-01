using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectIvy.Model.Converters;

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    private const string AlternativeDateTimeMsFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
    private const string AlternativeDateTimeMsNoZFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
    private const string AlternativeDateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
    private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    private const string DateFormat = "yyyy-MM-dd";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string value = reader.GetString();
        try
        {
            return DateTime.ParseExact(value, DateTimeFormat, CultureInfo.InvariantCulture);
        }
        catch
        {
            try
            {
                return DateTime.ParseExact(value, DateFormat, CultureInfo.InvariantCulture);
            }
            catch
            {
                try
                {
                    return DateTime.ParseExact(value, AlternativeDateTimeMsFormat, CultureInfo.InvariantCulture);
                }
                catch
                {
                    try
                    {
                        return DateTime.ParseExact(value, AlternativeDateTimeFormat, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        return DateTime.ParseExact(value, AlternativeDateTimeMsNoZFormat, CultureInfo.InvariantCulture);
                    }
                }
            }
        }
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateTimeFormat, CultureInfo.InvariantCulture));
    }
}