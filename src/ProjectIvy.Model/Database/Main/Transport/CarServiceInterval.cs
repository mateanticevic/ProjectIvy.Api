using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Transport;

[Table(nameof(CarServiceInterval), Schema = nameof(Transport))]
public class CarServiceInterval
{
    public int CarModelId { get; set; }

    public int CarServiceTypeId { get; set; }

    public short? Days { get; set; }

    public int? Range { get; set; }

    public CarModel CarModel { get; set; }

    public CarServiceType CarServiceType { get; set; }
}
