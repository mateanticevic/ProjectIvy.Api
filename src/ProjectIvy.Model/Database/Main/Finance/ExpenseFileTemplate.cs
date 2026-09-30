using System.ComponentModel.DataAnnotations.Schema;
using ProjectIvy.Model.Database.Main.Common;

namespace ProjectIvy.Model.Database.Main.Finance;

[Table(nameof(ExpenseFileTemplate), Schema = nameof(Finance))]
public class ExpenseFileTemplate : UserEntity, IHasName, IHasValueId
{
    public string AmountRegex { get; set; }

    public string CommentTemplate { get; set; }

    public Currency Currency { get; set; }

    public int? CurrencyId { get; set; }

    public string DayRegex { get; set; }

    public int? DefaultDayOfMonth { get; set; }

    public ExpenseType ExpenseType { get; set; }

    public int ExpenseTypeId { get; set; }

    public string FileType { get; set; }

    public int Id { get; set; }

    public string MatchRegex { get; set; }

    public string MonthRegex { get; set; }

    public string Name { get; set; }

    public PaymentType PaymentType { get; set; }

    public int PaymentTypeId { get; set; }

    public string ValueId { get; set; }

    public Vendor Vendor { get; set; }

    public int? VendorId { get; set; }

    public string YearRegex { get; set; }
}
