using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Income
{
    public class IncomeType
    {
        public IncomeType(DatabaseModel.Finance.IncomeType x)
        {
            Id = x.ValueId;
            Name = x.Name;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}
