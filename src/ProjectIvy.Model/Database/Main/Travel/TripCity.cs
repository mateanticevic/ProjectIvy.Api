using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Travel
{
    [Table(nameof(TripCity), Schema = nameof(Travel))]
    public class TripCity
    {
        public int CityId { get; set; }

        public int TripId { get; set; }

        public DateTime? EnteredOn { get; set; }

        public Common.City City { get; set; }

        public Trip Trip { get; set; }
    }
}
