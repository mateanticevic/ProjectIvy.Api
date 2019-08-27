using System;
using System.Collections.Generic;

namespace ProjectIvy.Model.Binding.Income
{
    public class IncomeGetBinding : FilteredPagedBinding
    {
        public IEnumerable<DayOfWeek> Day { get; set; }

        public IncomeSort OrderBy { get; set; }

        public string CurrencyId { get; set; }

        public string SourceId { get; set; }

        public string TypeId { get; set; }
    }
}
