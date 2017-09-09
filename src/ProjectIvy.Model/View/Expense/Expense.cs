using ProjectIvy.Extensions.BuiltInTypes;
using DatabaseModel = ProjectIvy.Model.Database.Main;
using System;

namespace ProjectIvy.Model.View.Expense
{
    public class Expense
    {
        public Expense(DatabaseModel.Finance.Expense x)
        {
            Amount = x.Ammount;
            Card = x.Card.ConvertTo(y => new Card.Card(y));
            Comment = x.Comment;
            Currency = x.Currency.ConvertTo(y => new Currency.Currency(y));
            Date = x.Date;
            ExpenseType = x.ExpenseType.ConvertTo(y => new ExpenseType.ExpenseType(y));
            Id = x.ValueId;
            PaymentType = x.PaymentType.ConvertTo(y => new PaymentType.PaymentType(y));
            Poi = x.Poi.ConvertTo(y => new Poi.Poi(y));
            Vendor = x.Vendor.ConvertTo(y => new Vendor.Vendor(y));
        }

        public decimal Amount { get; set; }

        public string Comment { get; set; }

        public Card.Card Card { get;set;}

        public Currency.Currency Currency { get; set; }

        public DateTime Date { get; set; }

        public ExpenseType.ExpenseType ExpenseType { get; set; }

        public Poi.Poi Poi { get; set; }

        public string Id { get; set; }

        public PaymentType.PaymentType PaymentType { get; set; }

        public Vendor.Vendor Vendor { get; set; }
    }
}
