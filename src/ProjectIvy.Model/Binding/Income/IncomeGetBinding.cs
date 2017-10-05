using ProjectIvy.Model.Binding.Common;

namespace ProjectIvy.Model.Binding.Income
{
    public class IncomeGetBinding : FilteredPagedBinding
    {
        public IncomeSort OrderBy { get; set; }
    }
}
