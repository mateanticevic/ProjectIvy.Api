namespace ProjectIvy.Model.Binding.Expense
{
    public class ExpenseSumGetBinding : ExpenseGetBinding, IOrderable<GroupedSort>
    {
        public ExpenseSumGetBinding() { }

        public ExpenseSumGetBinding(FilteredBinding binding) : base(binding) { }

        public new GroupedSort OrderBy { get; set; } = GroupedSort.Date;

        public string TargetCurrencyId { get; set; }

        public ExpenseSumGetBinding Override(string typeId)
        {
            TypeId = new[] { typeId };

            return this;
        }

        public ExpenseSumGetBinding OverrideMonth(int month)
        {
            Month = new int[] { month };

            return this;
        }
    }
}
