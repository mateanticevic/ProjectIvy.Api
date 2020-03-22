using ProjectIvy.Common.Extensions;

namespace ProjectIvy.Model.View.Car
{
    public class CarModel
    {
        public CarModel(Database.Main.Transport.CarModel c)
        {
            EngineDisplacement = c.EngineDisplacement;
            Name = c.Name;
            Power = c.Power;
            Manufacturer = c.ConvertTo(x => new Manufacturer(x.Manufacturer));
        }

        public short EngineDisplacement { get; set; }

        public string Name { get; set; }

        public short Power { get; set; }

        public Manufacturer Manufacturer { get; set; }
    }
}
