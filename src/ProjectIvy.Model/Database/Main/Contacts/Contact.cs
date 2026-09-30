using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Contacts;

[Table(nameof(Contact), Schema = nameof(Contacts))]
public class Contact : UserEntity, IHasCreatedModified
{
    public ContactType ContactType { get; set; }

    public int ContactTypeId { get; set; }

    public DateTime Created { get; set; }

    [Key]
    public int Id { get; set; }

    public string Identifier { get; set; }

    public DateTime Modified { get; set; }

    public Person Person { get; set; }

    public int PersonId { get; set; }
}
