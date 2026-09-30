using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.User;

[Table(nameof(ToDoTag), Schema = nameof(User))]
public class ToDoTag
{
    public int TagId { get; set; }

    public long ToDoId { get; set; }
}
