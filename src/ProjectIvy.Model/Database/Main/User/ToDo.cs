using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.User;

[Table(nameof(ToDo), Schema = nameof(User))]
public class ToDo : UserEntity, IHasCreated, IHasName
{
    [Key]
    public long Id { get; set; }

    public DateTime Created { get; set; }

    public string ValueId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }
}
