using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Travel;

[Table(nameof(CityVisited), Schema = nameof(Travel))]
public class CityVisited : UserEntity
{
    [Key]
    public int Id { get; set; }

    public int CityId { get; set; }

    public int? TripId { get; set; }

    public DateTime? Timestamp { get; set; }

    public Common.City City { get; set; }

    public Trip Trip { get; set; }
}
