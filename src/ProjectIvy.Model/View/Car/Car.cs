using ProjectIvy.Common.Extensions;
using System.Linq;

namespace ProjectIvy.Model.View.Car;

public class Car
{
    public Car(Database.Main.Transport.Car c)
    {
        Id = c.ValueId;
        Model = c.CarModel == null ? null : c.ConvertTo(x => new CarModel(x.CarModel));
        ProductionYear = c.ProductionYear;
        Services = c.CarServices.EmptyIfNull().OrderByDescending(x => x.Date).Select(x => x.ConvertTo(y => new CarService(y)));
    }

    public string Id { get; set; }

    public CarModel Model { get; set; }

    public short ProductionYear { get; set; }

    public IEnumerable<CarService> Services { get; set; }

    public IEnumerable<CarServiceDue> ServiceDue { get; set; }
}
