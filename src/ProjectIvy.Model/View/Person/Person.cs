using DatabaseModel = ProjectIvy.Model.Database.Main.Contacts;

namespace ProjectIvy.Model.View.Person;

public record Person(DatabaseModel.Person p)
{
    public string Id { get; set; } = p.ValueId;

    public string FirstName { get; set; } = p.FirstName;

    public string LastName { get; set; } = p.LastName;

    public DateTime DateOfBirth { get; set; } = p.DateOfBirth;
}
