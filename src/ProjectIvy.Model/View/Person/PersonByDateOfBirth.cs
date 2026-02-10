using System.Text.Json.Serialization;

namespace ProjectIvy.Model.View.Person;

public record PersonByDateOfBirth
{
    [JsonConverter(typeof(DateFormatConverter))]
    public DateTime DateOfBirth { get; set; }

    public IEnumerable<Person> People { get; set; }
}
