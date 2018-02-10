using ProjectIvy.Model.Constants.Database;

namespace ProjectIvy.Model.Binding.Consumation
{
    public class ConsumationGetBinding : FilteredPagedBinding
    {
        public string BeerId { get; set; }

        public string BeerBrandId { get; set; }

        public BeerServing? Serving { get; set; }
    }
}
