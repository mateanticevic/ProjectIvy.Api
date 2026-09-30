using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.User;

[Table(nameof(ToDo), Schema = nameof(User))]
public class ToDo : UserEntity, IHasCreated, IHasName
{
    public DateTime? CompletedOn { get; set; }

    public DateTime Created { get; set; }

    public int? CurrencyId { get; set; }

    public string Description { get; set; }

    public DateTime? DueDate { get; set; }

    public int? EstimatedPrice { get; set; }

    [Key]
    public long Id { get; set; }

    public bool IsCompleted { get; set; }

    public string Name { get; set; }

    public string ValueId { get; set; }
}
