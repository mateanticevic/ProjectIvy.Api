using System.ComponentModel.DataAnnotations.Schema;

namespace AnticevicApi.Model.Database.Main.Travel
{
    [Table(nameof(TripExpenseExclude), Schema = nameof(Travel))]
    public class TripExpenseExclude
    {
        public int ExpenseId { get; set; }

        public int TripId { get; set; }

        public Finance.Expense Expense { get; set; }

        public Trip Trip { get; set; }
    }
}
