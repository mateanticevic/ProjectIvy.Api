using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Tracking;

[Table("TrackingDistance", Schema = "Tracking")]
public class TrackingDistance : UserEntity, IHasTimestamp
{
    public int DistanceInMeters { get; set; }

    public DateTime Timestamp { get; set; }
}
