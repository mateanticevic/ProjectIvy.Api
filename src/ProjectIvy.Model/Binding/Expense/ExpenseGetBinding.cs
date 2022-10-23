namespace ProjectIvy.Model.Binding.Expense
{
    public class ExpenseGetBinding : FilteredPagedBinding, IOrderable<ExpenseSort>
    {
        public ExpenseGetBinding() { }

        public ExpenseGetBinding(FilteredBinding binding)
        {
            From = binding.From;
            To = binding.To;
        }

        public IEnumerable<DayOfWeek> Day { get; set; }

        public IEnumerable<int> Month { get; set; }

        public virtual ExpenseSort OrderBy { get; set; }

        public bool? HasLinkedFiles { get; set; }

        public bool? HasPoi { get; set; }

        public bool? NeedsReview { get; set; }

        public decimal? AmountFrom { get; set; }

        public decimal? AmountTo { get; set; }

        public IEnumerable<string> CardId { get; set; }

        public IEnumerable<string> CurrencyId { get; set; }

        public string Description { get; set; }

        public IEnumerable<string> PaymentTypeId { get; set; }

        public IEnumerable<string> TypeId { get; set; }

        public IEnumerable<string> VendorId { get; set; }

        public IEnumerable<string> ExcludeId { get; set; }
    }
}
