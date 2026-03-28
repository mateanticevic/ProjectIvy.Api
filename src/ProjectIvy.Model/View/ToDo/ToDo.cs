namespace ProjectIvy.Model.View.ToDo;

public class ToDo
{
    public ToDo(Database.Main.User.ToDo x)
    {
        Id = x.ValueId;
        Created = x.Created;
        Name = x.Name;
        Description = x.Description;
        EstimatedPrice = x.EstimatedPrice;
    }

    public string Id { get; set; }

    public DateTime Created { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int? EstimatedPrice { get; set; }

    public Currency.Currency Currency { get; set; }

    public IEnumerable<Tag.Tag> Tags { get; set; }

    public IEnumerable<Trip.Trip> Trips { get; set; }
}
