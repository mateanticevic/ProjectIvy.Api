using System.Linq;
using ProjectIvy.Common.Extensions;
using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Expense;

public class Expense
{
    public Expense(DatabaseModel.Finance.Expense x)
    {
        Amount = x.Amount;
        ExternalId = x.ExternalId;
        Card = x.Card.ConvertTo(y => new Card.Card(y));
        Comment = x.Comment;
        Currency = x.Currency.ConvertTo(y => new Currency.Currency(y));
        Files = x.ExpenseFiles?.Select(y => y.ConvertTo(p => new ExpenseFile(p)));
        ParentCurrency = x.ParentCurrency?.ConvertTo(y => new Currency.Currency(y));
        Date = x.Date;
        DatePaid = x.DatePaid;
        ExpenseType = x.ExpenseType.ConvertTo(y => new ExpenseType.ExpenseType(y));
        Id = x.ValueId;
        PaymentType = x.PaymentType.ConvertTo(y => new PaymentType.PaymentType(y));
        Poi = x.Poi.ConvertTo(y => new Poi.Poi(y));
        Vendor = x.Vendor.ConvertTo(y => new Vendor.Vendor(y));
        ParentAmount = x.ParentAmount;
        ParentCurrencyExchangeRate = x.ParentCurrencyExchangeRate;
        Modified = x.Modified;
        Timestamp = x.Created;
        NeedsReview = x.NeedsReview;
        InstallmentRef = x.InstallmentRef;
    }

    public decimal Amount { get; set; }

    public Card.Card Card { get; set; }

    public string Comment { get; set; }

    public Currency.Currency Currency { get; set; }

    public DateTime Date { get; set; }

    public DateTime DatePaid { get; set; }

    public ExpenseType.ExpenseType ExpenseType { get; set; }

    public string ExternalId { get; set; }

    public IEnumerable<ExpenseFile> Files { get; set; }

    public string Id { get; set; }

    public string InstallmentRef { get; set; }

    public DateTime? Modified { get; set; }

    public string Name => $"[{ExpenseType.Name}] {Comment} ({Amount} {Currency.Code})";

    public bool NeedsReview { get; set; }

    public decimal? ParentAmount { get; set; }

    public Currency.Currency ParentCurrency { get; set; }

    public decimal? ParentCurrencyExchangeRate { get; set; }

    public PaymentType.PaymentType PaymentType { get; set; }

    public Poi.Poi Poi { get; set; }

    public DateTime? Timestamp { get; set; }

    public Vendor.Vendor Vendor { get; set; }
}
