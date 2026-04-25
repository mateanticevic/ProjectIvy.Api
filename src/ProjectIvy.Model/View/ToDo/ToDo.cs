namespace ProjectIvy.Model.View.ToDo;

public class ToDo
{
    public ToDo(Database.Main.User.ToDo x)
    {
        Created = x.Created;
        Description = x.Description;
        DueDate = x.DueDate;
        EstimatedPrice = x.EstimatedPrice;
        Id = x.ValueId;
        IsCompleted = x.IsCompleted;
        Name = x.Name;
    }

    public string Id { get; set; }

    public DateTime Created { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime? DueDate { get; set; }

    public int? EstimatedPrice { get; set; }

    public Currency.Currency Currency { get; set; }

    public IEnumerable<Tag.Tag> Tags { get; set; }

    public IEnumerable<Trip.Trip> Trips { get; set; }
}
