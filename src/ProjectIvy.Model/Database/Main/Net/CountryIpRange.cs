using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Net;

[Table(nameof(CountryIpRange), Schema = nameof(Net))]
public class CountryIpRange
{
    [Key]
    public int Id { get; set; }

    public int CountryId { get; set; }

    public long FromIpValue { get; set; }

    public long ToIpValue { get; set; }

    public Common.Country Country { get; set; }
}
