using AnticevicApi.Model.Binding.Common;

namespace AnticevicApi.Model.Binding.Expense
{
    public class ExpenseGetBinding : FilteredPagedBinding
    {
        public string CurrencyId { get; set; }
        public string TypeId { get; set; }
        public string VendorId { get; set; }
    }
}
