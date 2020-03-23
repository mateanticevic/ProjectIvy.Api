namespace ProjectIvy.Model.View.Car
{
    public class CarServiceType
    {
        public CarServiceType(Database.Main.Transport.CarServiceType c)
        {
            Name = c.Name;
            Id = c.ValueId;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}
