using DatabaseModel = ProjectIvy.Model.Database.Main;
using ProjectIvy.Common.Extensions;

namespace ProjectIvy.Model.View.ExpenseType
{
    public class ExpenseTypeCount
    {
        public ExpenseTypeCount(DatabaseModel.Finance.ExpenseType t, int count)
        {
            Count = count;
            Type = t.ConvertTo(x => new ExpenseType(x));
        }

        public int Count { get; set; }

        public ExpenseType Type { get; set; }
    }
}
