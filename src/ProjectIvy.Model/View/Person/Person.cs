using System.Text.Json.Serialization;
using DatabaseModel = ProjectIvy.Model.Database.Main.Contacts;

namespace ProjectIvy.Model.View.Person;

public record Person
{
    public Person(DatabaseModel.Person p)
    {
        Id = p.ValueId;
        FirstName = p.FirstName;
        LastName = p.LastName;
        DateOfBirth = p.DateOfBirth;
    }
    
    public string Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    [JsonConverter(typeof(DateFormatConverter))]
    public DateTime DateOfBirth { get; set; }
}
