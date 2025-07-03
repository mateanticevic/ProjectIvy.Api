using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Tracking;

[Table(nameof(RoutePoint), Schema = nameof(Tracking))]
public class RoutePoint
{
    public int RouteId { get; set; }

    public int Index { get; set; }

    public decimal Lat { get; set; }

    public decimal Lng { get; set; }

    public string Geohash { get; set; }

    public Route Route { get; set; }
}
