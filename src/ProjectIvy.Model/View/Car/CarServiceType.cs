namespace ProjectIvy.Model.View.Car;

public class CarServiceType
{
    public CarServiceType(Database.Main.Transport.CarServiceType c)
    {
        Id = c.ValueId;
        Name = c.Name;
    }

    public string Id { get; set; }

    public string Name { get; set; }
}
