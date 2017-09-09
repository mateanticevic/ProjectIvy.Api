using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Income
{
    public class IncomeSource
    {
        public IncomeSource(DatabaseModel.Finance.IncomeSource x)
        {
            ValueId = x.ValueId;
            Name = x.Name;
        }

        public string ValueId { get; set; }

        public string Name { get; set; }
    }
}
