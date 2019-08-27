using ProjectIvy.Common.Extensions;
using ProjectIvy.Model.Constants.Database;
using System;
using DatabaseModel = ProjectIvy.Model.Database.Main.Beer;

namespace ProjectIvy.Model.View.Consumation
{
    public class Consumation
    {
        public Consumation(DatabaseModel.Consumation c)
        {
            Beer = c.Beer.ConvertTo(x => new Beer.Beer(x));
            Date = c.Date;
            Serving = (BeerServing)c.BeerServingId;
            Volume = c.Volume;
        }

        public Beer.Beer Beer { get; set; }

        public DateTime Date { get; set; }

        public int Volume { get; set; }

        public BeerServing Serving { get; set; }
    }
}
