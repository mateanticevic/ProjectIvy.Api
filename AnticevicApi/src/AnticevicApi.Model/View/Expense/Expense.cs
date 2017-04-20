using AnticevicApi.Extensions.BuiltInTypes;
using DatabaseModel = AnticevicApi.Model.Database.Main;
using System;

namespace AnticevicApi.Model.View.Expense
{
    public class Expense
    {
        public Expense(DatabaseModel.Finance.Expense x)
        {
            Amount = x.Ammount;
            Comment = x.Comment;
            Currency = x.Currency.ConvertTo(y => new Currency.Currency(y));
            Date = x.Date;
            ExpenseType = x.ExpenseType.ConvertTo(y => new ExpenseType.ExpenseType(y));
            ValueId = x.ValueId;
            Vendor = x.Vendor.ConvertTo(y => new Vendor.Vendor(y));
        }

        public decimal Amount { get; set; }

        public string Comment { get; set; }

        public Currency.Currency Currency { get; set; }

        public DateTime Date { get; set; }

        public ExpenseType.ExpenseType ExpenseType { get; set; }

        public string ValueId { get; set; }

        public Vendor.Vendor Vendor { get; set; }
    }
}
