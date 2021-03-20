using System;

namespace ProjectIvy.Model.Binding.Expense
{
    public class ExpenseSumGetBinding : ExpenseGetBinding
    {
        public ExpenseSumGetBinding() { }

        public ExpenseSumGetBinding(FilteredBinding binding) : base(binding) { }

        public string TargetCurrencyId { get; set; }

        public ExpenseSumGetBinding Override(string typeId)
        {
            TypeId = new[] { typeId };

            return this;
        }

        public ExpenseSumGetBinding OverrideDay(DayOfWeek day)
        {
            Day = new DayOfWeek[] { day };

            return this;
        }

        public ExpenseSumGetBinding OverrideMonth(int month)
        {
            Month = new int[] { month };

            return this;
        }
    }
}
