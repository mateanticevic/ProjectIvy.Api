using System;
using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Car
{
    public class CarLog
    {
        public CarLog(DatabaseModel.Transport.CarLog x)
        {
            Odometer = x.Odometer;
            Timestamp = x.Timestamp;
        }

        public int? Odometer { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
