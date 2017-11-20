using ProjectIvy.Model.Binding.Common;

namespace ProjectIvy.Model.Binding.Expense
{
    public class ExpenseGetBinding : FilteredPagedBinding
    {
        public ExpenseSort OrderBy { get; set; }

        public bool? HasLinkedFiles { get; set; }

        public bool? HasPoi { get; set; }

        public decimal? AmountFrom { get; set; }

        public decimal? AmountTo { get; set; }

        public string CurrencyId { get; set; }

        public string Description { get; set; }

        public string TypeId { get; set; }

        public string VendorId { get; set; }
    }
}
