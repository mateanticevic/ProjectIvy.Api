using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Travel;

[Table(nameof(TripTodo), Schema = nameof(Travel))]
public class TripTodo
{
    public long ToDoId { get; set; }

    public int TripId { get; set; }
}
