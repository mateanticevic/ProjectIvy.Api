using ProjectIvy.Common.Extensions;

namespace ProjectIvy.Model.View.Car;

public class CarServiceInterval
{
    public CarServiceInterval(Database.Main.Transport.CarServiceInterval c)
    {
        Days = c.Days;
        Range = c.Range;
        ServiceType = c.ConvertTo(x => new CarServiceType(x.CarServiceType));
    }

    public short? Days { get; set; }

    public int? Range { get; set; }

    public CarServiceType ServiceType { get; set; }
}
