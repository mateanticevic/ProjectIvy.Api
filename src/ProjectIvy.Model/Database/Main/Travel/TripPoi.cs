using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Travel;

[Table(nameof(TripPoi), Schema = nameof(Travel))]
public class TripPoi
{
    public int PoiId { get; set; }

    public int TripId { get; set; }

    public Poi Poi { get; set; }

    public Trip Trip { get; set; }
}
