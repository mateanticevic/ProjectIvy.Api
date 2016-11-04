using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.Income
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
