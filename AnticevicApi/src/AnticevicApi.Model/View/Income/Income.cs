using AnticevicApi.Extensions.BuiltInTypes;
using DatabaseModel = AnticevicApi.Model.Database.Main;
using System;

namespace AnticevicApi.Model.View.Income
{
    public class Income
    {
        public Income(DatabaseModel.Finance.Income x)
        {
            Amount = x.Ammount;
            Currency = x.ConvertTo(y => new Currency.Currency(y.Currency));
            Description = x.Description;
            Source = x.IncomeSource.ConvertTo(y => new IncomeSource(y));
            Type = x.IncomeType.ConvertTo(y => new IncomeType(y));
            Timestamp = x.Timestamp;
        }

        public decimal Amount { get; set; }

        public Currency.Currency Currency { get; set; }

        public string Description { get; set; }

        public IncomeSource Source { get; set; }

        public IncomeType Type { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
