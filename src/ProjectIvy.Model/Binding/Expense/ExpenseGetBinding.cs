using ProjectIvy.Model.Binding.Route;

namespace ProjectIvy.Model.Binding.Expense;

public class ExpenseGetBinding : FilteredPagedBinding, IOrderable<ExpenseSort>, ISearchable
{
    public ExpenseGetBinding() { }

    public ExpenseGetBinding(FilteredBinding binding)
    {
        From = binding.From;
        To = binding.To;
    }

    public decimal? AmountFrom { get; set; }

    public decimal? AmountTo { get; set; }

    public IEnumerable<string> CardId { get; set; }

    public IEnumerable<string> CurrencyId { get; set; }

    public IEnumerable<DayOfWeek> Day { get; set; }

    public string Description { get; set; }

    public IEnumerable<string> ExcludeId { get; set; }

    public IEnumerable<string> ExcludeTypeId { get; set; }

    public IEnumerable<string> ExternalId { get; set; }

    public bool? HasLinkedFiles { get; set; }

    public bool? HasPoi { get; set; }

    public IEnumerable<int> Month { get; set; }

    public bool? NeedsReview { get; set; }

    public virtual ExpenseSort OrderBy { get; set; }

    public IEnumerable<string> PaymentTypeId { get; set; }

    public string Search { get; set; }

    public IEnumerable<string> TypeId { get; set; }

    public IEnumerable<string> VendorId { get; set; }
}
