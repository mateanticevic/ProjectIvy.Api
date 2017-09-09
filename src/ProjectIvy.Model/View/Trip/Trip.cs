using ProjectIvy.Common.Extensions;
using DatabaseModel = ProjectIvy.Model.Database.Main;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ProjectIvy.Model.View.Trip
{
    public class Trip
    {
        public Trip(DatabaseModel.Travel.Trip x)
        {
            Id = x.ValueId;
            Cities = x.Cities.Select(y => new City.City(y.City));
            Countries = x.Cities.Select(y => new Country.Country(y.City.Country))
                                .GroupBy(y => y.Id)
                                .Select(y => y.FirstOrDefault());
            Name = x.Name;
            Pois = x.Pois.IsNullOrEmpty() ? null : x.Pois.Select(y => new Poi.Poi(y.Poi));
            TimestampEnd = x.TimestampEnd;
            TimestampStart = x.TimestampStart;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime TimestampEnd { get; set; }

        public DateTime TimestampStart { get; set; }

        public int Distance { get; set; }

        public IEnumerable<Country.Country> Countries { get; set; }

        public IEnumerable<City.City> Cities { get; set; }

        public IEnumerable<Expense.Expense> Expenses { get; set; }

        public IEnumerable<Poi.Poi> Pois { get; set; }

        public decimal TotalSpent { get; set; }
    }
}
