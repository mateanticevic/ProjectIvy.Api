using System;

namespace ProjectIvy.Model.View.Car
{
    public class CarServiceDue
    {
        public int? DueAt { get; set; }

        public DateTime? DueBefore { get; set; }

        public CarServiceType ServiceType { get; set; }
    }
}
