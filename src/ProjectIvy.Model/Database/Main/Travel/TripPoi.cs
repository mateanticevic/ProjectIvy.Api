using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Travel;

[Table(nameof(TripPoi), Schema = nameof(Travel))]
public class TripPoi
{
    public Poi Poi { get; set; }

    public int PoiId { get; set; }

    public Trip Trip { get; set; }

    public int TripId { get; set; }
}
