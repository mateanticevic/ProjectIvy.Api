using ProjectIvy.Common.Extensions;

namespace ProjectIvy.Model.View.Car
{
    public class Car
    {
        public Car(Database.Main.Transport.Car c)
        {
            Id = c.ValueId;
            Model = c.ConvertTo(x => new CarModel(x.CarModel));
            ProductionYear = c.ProductionYear;
        }

        public string Id { get; set; }

        public CarModel Model { get; set; }

        public short ProductionYear { get; set; }
    }
}
