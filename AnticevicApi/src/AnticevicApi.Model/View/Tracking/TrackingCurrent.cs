﻿using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.Tracking
{
    public class TrackingCurrent
    {
        public TrackingCurrent(DatabaseModel.Tracking.Tracking x)
        {
            Latitude = x.Latitude;
            Longitude = x.Longitude;
        }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }
    }
}
