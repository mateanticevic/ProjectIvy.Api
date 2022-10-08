using ProjectIvy.Common.Extensions;

namespace ProjectIvy.Model.View.Car
{
    public class CarModel
    {
        public CarModel(Database.Main.Transport.CarModel c)
        {
            EngineDisplacement = c.EngineDisplacement;
            ModelYear = c.ModelYear;
            Id = c.ValueId;
            Name = c.Name;
            Power = c.Power;
            Manufacturer = c.Manufacturer == null ? null : c.ConvertTo(x => new Manufacturer(x.Manufacturer));
        }

        public short EngineDisplacement { get; set; }

        public short ModelYear { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public short Power { get; set; }

        public Manufacturer Manufacturer { get; set; }
    }
}
