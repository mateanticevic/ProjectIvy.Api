using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Transport;

[Table(nameof(RideType), Schema = nameof(Transport))]
public class RideType : IHasValueId, IHasName
{
    [Key]
    public int Id { get; set; }

    public string ValueId { get; set; }

    public string Name { get; set; }
}
