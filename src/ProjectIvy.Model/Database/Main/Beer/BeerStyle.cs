using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Beer;

[Table(nameof(BeerStyle), Schema = nameof(Beer))]
public class BeerStyle : IHasName, IHasValueId
{
    [Key]
    public int Id { get; set; }

    public string ValueId { get; set; }

    public string Name { get; set; }
}
