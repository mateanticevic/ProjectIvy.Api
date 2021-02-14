using System;
namespace ProjectIvy.Model.Binding.Income
{
    public class IncomeBinding
    {
        public decimal Amount { get; set; }

        public string CurrencyId { get; set; }

        public string Description { get; set; }

        public string SourceId { get; set; }

        public string TypeId { get; set; }

        public DateTime Date { get; set; }
    }
}
