using ProjectIvy.Model.Constants.Database;

namespace ProjectIvy.Model.Binding.Consumation
{
    public class ConsumationGetBinding : FilteredPagedBinding
    {
        public ConsumationGetBinding() { }

        public ConsumationGetBinding(FilteredBinding binding)
        {
            From = binding.From;
            To = binding.To;
        }

        public string BeerId { get; set; }

        public string BrandId { get; set; }

        public BeerServing? Serving { get; set; }
    }
}
