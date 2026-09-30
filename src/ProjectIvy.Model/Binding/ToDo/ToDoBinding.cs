namespace ProjectIvy.Model.Binding.ToDo;

public class ToDoBinding
{
    public string CurrencyId { get; set; }

    public string Description { get; set; }

    public DateTime? DueDate { get; set; }

    public int? EstimatedPrice { get; set; }

    public bool IsCompleted { get; set; }

    public string Name { get; set; }

    public IEnumerable<string> TagIds { get; set; }
}
