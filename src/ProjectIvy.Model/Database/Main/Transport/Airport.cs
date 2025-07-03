using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Transport;

[Table(nameof(Airport), Schema = nameof(Transport))]
public class Airport
{
    [Key]
    public int Id { get; set; }

    public string Iata { get; set; }

    public string Name { get; set; }

    public int CityId { get; set; }

    public int PoiId { get; set; }

    public Common.City City { get; set; }

    public Travel.Poi Poi { get; set; }
}
