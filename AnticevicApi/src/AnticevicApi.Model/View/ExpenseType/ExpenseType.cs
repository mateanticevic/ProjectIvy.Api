using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.ExpenseType
{
    public class ExpenseType
    {
        public ExpenseType(DatabaseModel.Finance.ExpenseType x)
        {
            Icon = x.Icon;
            IconUrl = x.IconUrl;
            ValueId = x.ValueId;
            Name = x.TypeDescription;
        }
        public string Icon { get; set; }
        public string IconUrl { get; set; }
        public string Name { get; set; }
        public string ValueId { get; set; }
    }
}
