using DatabaseModel = AnticevicApi.Model.Database.Main;
using System.Collections.Generic;
using System;

namespace AnticevicApi.Model.View.Trip
{
    public class Trip
    {
        public Trip(DatabaseModel.Travel.Trip x)
        {
            Id = x.ValueId;
            Name = x.Name;
            TimestampEnd = x.TimestampEnd;
            TimestampStart = x.TimestampStart;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime TimestampEnd { get; set; }

        public DateTime TimestampStart { get; set; }

        public IEnumerable<Expense.Expense> Expenses { get; set; }
    }
}
