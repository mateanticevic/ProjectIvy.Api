using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Transport;

[Table(nameof(CarServiceType), Schema = nameof(Transport))]
public class CarServiceType : IHasValueId
{
    [Key]
    public int Id { get; set; }

    public string ValueId { get; set; }

    public string Name { get; set; }

    public ICollection<CarService> CarServices { get; set; }
}
