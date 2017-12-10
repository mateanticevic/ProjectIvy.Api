namespace ProjectIvy.Model.Binding.Expense
{
    public class ExpenseSumGetBinding : ExpenseGetBinding, IOrderable<GroupedSort>
    {
        public new GroupedSort OrderBy { get; set; } = GroupedSort.Date;

        public string TargetCurrencyId { get; set; }

        public ExpenseSumGetBinding Override(string typeId)
        {
            TypeId = typeId;

            return this;
        }
    }
}
