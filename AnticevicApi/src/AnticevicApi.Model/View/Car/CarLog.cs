using DatabaseModel = AnticevicApi.Model.Database.Main;
using System;

namespace AnticevicApi.Model.View.Car
{
    public class CarLog
    {
        public CarLog(DatabaseModel.Transport.CarLog x)
        {
            Odometer = x.Odometer;
            Timestamp = x.Timestamp;
        }

        public int Odometer { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
