using ProjectIvy.Model.Constants.Database;

namespace ProjectIvy.Model.Binding.Consumation
{
    public class ConsumationGetBinding : FilteredPagedBinding
    {
        public BeerServing? Serving { get; set; }
    }
}
