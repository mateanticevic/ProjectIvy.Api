using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Contacts;

[Table(nameof(ContactType), Schema = nameof(Contacts))]
public class ContactType : IHasName, IHasValueId
{
    [Key]
    public int Id { get; set; }

    public string ValueId { get; set; }

    public string Name { get; set; }
}
