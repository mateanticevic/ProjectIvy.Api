namespace ProjectIvy.Model.View.Car;

public class Manufacturer
{
    public Manufacturer(Database.Main.Transport.Manufacturer c)
    {
        Name = c.Name;
    }

    public string Name { get; set; }
}
