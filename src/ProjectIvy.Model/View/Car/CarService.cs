using ProjectIvy.Common.Extensions;
using System;

namespace ProjectIvy.Model.View.Car
{
    public class CarService
    {
        public CarService(Database.Main.Transport.CarService c)
        {
            Date = c.Date;
            Description = c.Description;
            Id = c.ValueId;
            ServiceType = c.ConvertTo(x => new CarServiceType(x.CarServiceType));
        }

        public string Id { get; set; }

        public CarServiceType ServiceType { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }
    }
}
