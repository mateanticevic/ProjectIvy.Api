using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Income
{
    public class IncomeType
    {
        public IncomeType(DatabaseModel.Finance.IncomeType x)
        {
            ValueId = x.ValueId;
            Name = x.Name;
        }

        public string ValueId { get; set; }

        public string Name { get; set; }
    }
}
