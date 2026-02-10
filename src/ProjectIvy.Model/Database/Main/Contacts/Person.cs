using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Contacts;

[Table(nameof(Person), Schema = nameof(Contacts))]
public class Person : UserEntity, IHasCreatedModified, IHasValueId
{
    [Key]
    public int Id { get; set; }

    public string ValueId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime Created { get; set; }

    public DateTime DateOfBirth { get; set; }

    public DateTime Modified { get; set; }

    public ICollection<Contact> Contacts { get; set; }
}
