using ProjectIvy.Model.Binding.Common;

namespace ProjectIvy.Model.Binding.Income
{
    public class IncomeGetBinding : FilteredPagedBinding
    {
        public IncomeSort OrderBy { get; set; }

        public string CurrencyId { get; set; }

        public string SourceId { get; set; }

        public string TypeId { get; set; }
    }
}
