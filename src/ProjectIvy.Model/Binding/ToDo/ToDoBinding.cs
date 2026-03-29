namespace ProjectIvy.Model.Binding.ToDo;

public class ToDoBinding
{
    public string Name { get; set; }

    public string Description { get; set; }

    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; }

    public int? EstimatedPrice { get; set; }

    public string CurrencyId { get; set; }

    public IEnumerable<string> TagIds { get; set; }
}
