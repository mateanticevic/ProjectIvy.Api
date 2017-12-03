using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.ExpenseType
{
    public class ExpenseType
    {
        public ExpenseType(DatabaseModel.Finance.ExpenseType x)
        {
            Icon = x.Icon;
            Id = x.ValueId;
            Name = x.Name;
        }

        public string Id { get; set; }

        public string Icon { get; set; }

        public string Name { get; set; }
    }
}
