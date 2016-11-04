using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace AnticevicApi.Model.Database.Main.Tracking
{
    [Table("TrackingDistance", Schema = "Tracking")]
    public class TrackingDistance : UserEntity, IHasTimestamp
    {
        public DateTime Timestamp { get; set; }

        public int DistanceInMeters { get; set; }
    }
}
