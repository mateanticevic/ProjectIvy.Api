namespace ProjectIvy.Model.View.Car
{
    public class Car
    {
        public Car(Database.Main.Transport.Car c)
        {
            Id = c.ValueId;
            Model = c.Model;
            ProductionYear = c.ProductionYear;
        }

        public string Id { get; set; }

        public string Model { get; set; }

        public short ProductionYear { get; set; }
    }
}
