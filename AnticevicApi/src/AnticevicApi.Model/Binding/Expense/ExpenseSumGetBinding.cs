using AnticevicApi.Model.Binding.Common;

namespace AnticevicApi.Model.Binding.Expense
{
    public class ExpenseSumGetBinding : FilteredBinding
    {
        public ExpenseSumGetBinding Override(FilteredBinding binding)
        {
            From = binding.From;
            To = binding.To;

            return this;
        }

        public string CurrencyId { get; set; }
        public string TypeId { get; set; }
        public string VendorId { get; set; }
    }
}
