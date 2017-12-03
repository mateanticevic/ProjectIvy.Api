namespace ProjectIvy.Model.Binding.Expense
{
    public class ExpenseSumGetBinding : ExpenseGetBinding
    {
        public string TargetCurrencyId { get; set; }

        public ExpenseSumGetBinding Override(string typeId)
        {
            TypeId = typeId;

            return this;
        }
    }
}
