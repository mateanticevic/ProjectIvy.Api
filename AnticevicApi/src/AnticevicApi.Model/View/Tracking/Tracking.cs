﻿using AnticevicApi.Extensions.BuiltInTypes;
using DatabaseModel = AnticevicApi.Model.Database.Main;
using System;

namespace AnticevicApi.Model.View.Tracking
{
    public class Tracking
    {
        public Tracking(DatabaseModel.Tracking.Tracking x)
        {
            Accuracy = x.Accuracy;
            Altitude = x.Altitude;
            Lat = x.Latitude;
            Lng = x.Longitude;
            Speed = x.Speed;
            Timestamp = x.Timestamp;
            User = x.User.ConvertTo(y => new User.User(y));
        }

        public double? Accuracy { get; set; }

        public double? Altitude { get; set; }

        public decimal Lat { get; set; }

        public decimal Lng { get; set; }

        public double? Speed { get; set; }

        public DateTime Timestamp { get; set; }

        public User.User User { get; set; }
    }
}
