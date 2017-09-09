using ProjectIvy.Model.Binding.Common;

namespace ProjectIvy.Model.Binding.Expense
{
    public class ExpenseSumGetBinding : FilteredBinding
    {
        public string CurrencyId { get; set; }

        public string TypeId { get; set; }

        public string VendorId { get; set; }

        public ExpenseSumGetBinding Override(FilteredBinding binding)
        {
            From = binding.From;
            To = binding.To;

            return this;
        }

        public ExpenseSumGetBinding Override(string typeId)
        {
            TypeId = typeId;

            return this;
        }
    }
}
