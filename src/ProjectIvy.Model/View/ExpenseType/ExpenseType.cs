using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.ExpenseType
{
    public class ExpenseType
    {
        public ExpenseType(DatabaseModel.Finance.ExpenseType x)
        {
            Icon = x.Icon;
            IconUrl = x.IconUrl;
            Id = x.ValueId;
            Name = x.TypeDescription;
        }

        public string Id { get; set; }

        public string Icon { get; set; }

        public string IconUrl { get; set; }

        public string Name { get; set; }
    }
}
