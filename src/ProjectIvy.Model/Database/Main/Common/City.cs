using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectIvy.Model.Database.Main.Finance;

namespace ProjectIvy.Model.Database.Main.Common;

[Table(nameof(City), Schema = nameof(Common))]
public class City : IHasValueId, IHasName
{
    [Key]
    public int Id { get; set; }

    public string ValueId { get; set; }

    public string Name { get; set; }

    public string Administration { get; set; }

    public int CountryId { get; set; }

    public decimal? Lat { get; set; }

    public decimal? Lng { get; set; }

    public long? Population { get; set; }

    public Country Country { get; set; }

    public ICollection<Transport.Airport> Airports { get; set; }

    public ICollection<Vendor> Vendors { get; set; }

    public ICollection<Travel.Trip> Trips { get; set; }
}
